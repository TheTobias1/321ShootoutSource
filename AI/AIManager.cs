using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace SquadAI
{
    public class AIManager : MonoBehaviour
    {
        [System.Serializable]
        private class AIGroup
        {
            public List<AIController> group;

            public void Add(AIController ai)
            {
                foreach(AIController sqaudMember in group)
                {
                    AIBlackboard.Link(sqaudMember.Blackboard, ai.Blackboard);
                }
                group.Add(ai);
            }
        }

        public static AIManager SquadManager;

        private float initTime;

        public GameObject player;

        private CoverPoint[] coverPoints;
        public float coverRange;
        public float antiCoverRange;
        public float coverUpdateDelayTime;

        public List<AIController> AISquad;
        private List<AIGroup> AIGroups;
        bool calculatingGroups;

        private bool calculatingCover;
        private bool calculatingAntiCover;

        bool trackingPlayer;
        public CoverPoint[] Cover
        {
            get
            {
                return coverPoints;
            }
        }

        IEnumerator coverRoutine;

        private bool playerStill;
        private Vector3 lastPosition;
        private float nextGrenadeThrow;
        private bool iteratingOverAis;
        public bool allowGrenades;

        public int aliveCount;
        public bool AllDead
        {
            get
            {
                return aliveCount <= 0;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            AIManager.SquadManager = this;
            initTime = Time.time;

            GameObject[] cover = GameObject.FindGameObjectsWithTag("Cover");
            coverPoints = new CoverPoint[cover.Length];
            for(int i = 0; i < cover.Length; ++i)
            {
                coverPoints[i] = cover[i].GetComponent<CoverPoint>();
                coverPoints[i].SetPlayer(player);
                coverPoints[i].Occupied = false;
            }

            #region Shuffle

            int n = coverPoints.Length;

            while (n > 1) 
            {
                int k = Random.Range(0, n--);
                CoverPoint temp = coverPoints[n];
                coverPoints[n] = coverPoints[k];
                coverPoints[k] = temp;
            }
            #endregion

            if (coverUpdateDelayTime > 0)
                StartCoroutine(UpdateCover());

            InvokeRepeating("TrackPlayerMovement", 3, 3);
            nextGrenadeThrow = Time.time + 5;
        }

        private void Update()
        {

            if(!iteratingOverAis)
            {
                iteratingOverAis = true;
                StartCoroutine(SquadIteration());
            }
        }

        IEnumerator UpdateCover()
        {
            AIController[] aiCover = AISquad.ToArray();
            AIController[] aiAnticover = aiCover;
            yield return null;

            int coverPointer = 0;
            int anticoverPointer = 0;

            for(int x = 0; x < coverPoints.Length; ++x)
            {
                CoverPoint c = coverPoints[x];
                c.TraceToPlayer();
                yield return new WaitForSeconds(coverUpdateDelayTime);

                Vector3 pos = c.transform.position;
                Vector3 playerPos = player.transform.position;

                /*Thread coverThread = new Thread(() => SuitCoverToAI(ref aiCover, ref coverPointer, c, pos, playerPos));
                if (coverPointer < aiCover.Length && !c.CanSeePlayer)
                {
                    coverThread.Start();
                }

                Thread antiCoverThread = new Thread(() => SuitAnticoverToAI(ref aiAnticover, ref anticoverPointer, c, pos, playerPos));
                if (anticoverPointer < aiAnticover.Length && c.CanSeePlayer)
                {
                    antiCoverThread.Start();
                }

                while (coverThread.IsAlive || antiCoverThread.IsAlive)
                {
                    yield return new WaitForSeconds(coverUpdateDelayTime);
                }*/

                #region Cover
                for(int i = coverPointer; i < aiCover.Length; ++i)
                {
                    if(aiCover[i] == null || c.Occupied)
                    {
                        continue;
                    }

                    if (aiCover[i].customAI)
                        continue;

                    if(aiCover[i].Blackboard.BestCover != null)
                    {
                        continue;
                    }

                    float dist = Vector3.Distance(pos, aiCover[i].currentPosition);
                    float dot = Vector3.Dot(Vector3.Normalize(playerPos - aiCover[i].currentPosition), Vector3.Normalize(pos - aiCover[i].currentPosition));
                    if (dist < coverRange && dot < 0.75f && !c.Occupied)//found cover
                    {
                        //swap it with the ai in the pointer position
                        AIController chosen = aiCover[i];
                        aiCover[i] = aiCover[coverPointer];
                        aiCover[coverPointer] = chosen;
                        ++coverPointer;

                        //occupy it
                        c.occupier = chosen;
                        chosen.Blackboard.BestCover = c;
                        
                        break;
                    }
                    yield return null;
                }
                #endregion

                #region  Anticover
                for (int i = anticoverPointer; i < aiAnticover.Length; ++i)
                {
                    if (aiAnticover[i] == null || c.Occupied)
                    {
                        continue;
                    }

                    if (aiAnticover[i].customAI)
                        continue;

                    if (aiAnticover[i].Blackboard.BestAntiCover != null)
                    {
                        continue;
                    }

                    float dot = Vector3.Dot(Vector3.Normalize(playerPos - aiAnticover[i].currentPosition), Vector3.Normalize(pos - aiAnticover[i].currentPosition));
                    float dist = Vector3.Distance(pos, playerPos);
                    float playerDist = Vector3.Distance(playerPos, aiAnticover[i].currentPosition);

                    //Debug.Log("O: " + c.Occupied + " D: " + playerDist.ToString());

                    if (dist < antiCoverRange && playerDist > 3 && !c.Occupied)//found cover
                    {
                        //swap it with the ai in the pointer position
                        AIController chosen = aiAnticover[i];
                        aiAnticover[i] = aiAnticover[anticoverPointer];
                        aiAnticover[anticoverPointer] = chosen;
                        ++anticoverPointer;

                        //occupy it
                        c.occupier = chosen;
                        chosen.Blackboard.BestAntiCover = c;
                        break;
                    }
                    yield return null;
                }
                #endregion
            }
            StartCoroutine(UpdateCover());
        }

        void LoadNewAISM(AIController ai, StateMachines s)
        {
            if(ai != null)
                ai.stateMachine.LoadNewSM(s);
        }

        public IEnumerator SquadIteration()
        {
            int count = 0;

            AIController[] squad = AISquad.ToArray();

            foreach(AIController ai in squad)
            {
                if(ai == null)
                {
                    continue;
                }

                ++count;

                if(!(ai is CustomAIController))
                {
                    CheckGrenades(ai);
                    PushOnLastAlive(ai);
                }


                yield return null;
            }

            aliveCount = count;
            iteratingOverAis = false;
        }

        void PushOnLastAlive(AIController ai)
        {
            if(aliveCount <= 2 && EnemySpawner.SPAWNING_ENEMIES)
            {
                if(ai.stateMachine.AIType != AIStatemachineManager.EnemyType.Pusher)
                {
                    ai.stateMachine.ReEvaluateAttack(AIStatemachineManager.EnemyType.Pusher);
                }
            }
        }

        void CheckGrenades(AIController ai)
        {
            if (playerStill && nextGrenadeThrow < Time.time && allowGrenades)
            {
                if (ai.canThrowGrenade && !ai.Blackboard.LostPlayer && !ai.components.locomotion.moveOverride && ai.Blackboard.Waiting)
                {
                    float d = Vector3.Distance(ai.transform.position, player.transform.position);

                    if (d < 25 && d > 7f)
                    {
                        //our guy
                        ai.components.locomotion.ThrowGrenade(player.transform);
                        nextGrenadeThrow = Time.time + 9;
                        Debug.Log("Grenade");
                    }
                }
            }
        }

        private void TrackPlayerMovement()
        {
            playerStill = Vector3.Distance(player.transform.position, lastPosition) < 5;
            lastPosition = player.transform.position;
        }

        public void SpawnNewAI(AIController newAI, Vector3 spawnPos, Quaternion rot)
        {
            AIController instance = Instantiate(newAI, spawnPos, rot);
            AISquad.Add(instance);
        }

    }
}

//OLD SYSTEMS

/* 
            if(trackingPlayer)
            {
                if(!AIBlackboard.PlayerSpotted)
                {
                    OnLostPlayer();
                    trackingPlayer = false;
                }
            }
            else
            {
                if (AIBlackboard.PlayerSpotted)
                {
                    trackingPlayer = true;
                }
            }
    
IEnumerator PlayerSpottedSequence()
{
calculatingGroups = true;

Vector3 playerPos = player.transform.position;
Thread groupThread = new Thread(() => GroupIntoSquads(playerPos));
groupThread.Start();

while(calculatingGroups)
    yield return null;

foreach(AIController ai in AIGroups[0].group)
{
    if (ai == null)
        continue;

    int r = Random.Range(0, 2);

    switch (r)
    {
        case 0:
            LoadNewAISM(ai, StateMachines.DefensiveAttack);
            break;      
        default:         
            LoadNewAISM(ai, StateMachines.GotoAndAttack);
            break;      
    }
}

Debug.Log("NEW STATES");

for(int i = 1; i < AIGroups.Count; ++i)
{
    if(AIGroups[i].group.Count > 1)
    {
        int j = 0;
        Transform lead = null;
        while (j < AIGroups[i].group.Count)
        {
            if(AIGroups[i].group[j] == null)
            {
                AIGroups[i].group.RemoveAt(j);
                continue;
            }

            if(j == 0)
            {
                lead = AIGroups[i].group[j].transform;
                LoadNewAISM(AIGroups[i].group[j], StateMachines.SquadApproach);
            }
            else if(j == 1)
            {
                SetSquadLeader(AIGroups[i].group[j], lead);
                LoadNewAISM(AIGroups[i].group[j], StateMachines.FrontCover);
            }
            else if (j == 2)
            {
                SetSquadLeader(AIGroups[i].group[j], lead);
                LoadNewAISM(AIGroups[i].group[j], StateMachines.BackCover);
            }
            ++j;
        }
    }
    else
    {
        if (AIGroups[i].group[0] == null)
            continue;

        int r = Random.Range(0, 2);

        switch(r)
        {
            case 0:
                LoadNewAISM(AIGroups[i].group[0], StateMachines.GotoAndAttack);
                break;
            default:
                LoadNewAISM(AIGroups[i].group[0], StateMachines.BlitzFlank);
                break;
        }
    }

}
}

void SetSquadLeader(AIController ai, Transform lead)
{
ai.Blackboard.squadLeader = lead;
}

public void GroupIntoSquads(Vector3 playerPosition)
{
AIGroups = new List<AIGroup>();
AIGroup newGroup = new AIGroup();
newGroup.group = new List<AIController>();

AIGroups.Add(newGroup);

AIController workingLeader = null;
AIGroup latestGroup = new AIGroup();
latestGroup.group = new List<AIController>();

int seeCount = 0;

for(int i = 0; i < AISquad.Count; ++i)
{
    if (AISquad[i] == null || !AISquad[i].Blackboard.Waiting)
    {
        Debug.Log("BAD AI");
        continue;
    }

    float dist = Vector3.Distance(playerPosition, AISquad[i].currentPosition);
    if(AISquad[i].Blackboard.CanSeePlayer || dist < 30)
    {
        AIGroups[0].Add(AISquad[i]);
        ++seeCount;
        continue;
    }

    if(workingLeader != null && latestGroup.group.Count < 3)
    {
        Vector3 leaderToPlayer = playerPosition - workingLeader.currentPosition;
        Vector3 currentToPlayer = playerPosition - AISquad[i].currentPosition;
        if (Vector3.Dot(leaderToPlayer.normalized, currentToPlayer.normalized) > -0.5f)
        {
            latestGroup.Add(AISquad[i]);
            continue;
        }
    }

    workingLeader = AISquad[i];
    latestGroup = new AIGroup();
    latestGroup.group = new List<AIController>();
    latestGroup.Add(workingLeader);
    AIGroups.Add(latestGroup);

}

calculatingGroups = false;
}*/
