using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class EnemySpawner : MonoBehaviour
    {
        public Transform playerStart;
        public AIManager squadManager;
        public EnemyWave[] Waves;
        public GameObject player;

        public int currentWave;

        private bool spawningEnemies;

        public static bool LEVEL_CLEAR;
        public static bool SPAWNING_ENEMIES;
        public static ActionDelegate OnLevelClear;

        public float spawnSafeDistance = 9;

        [SerializeField]
        private float difficultyMultiplier = 1;
        public bool useScalableDifficulty;

        private void Awake()
        {
            EnemySpawner.LEVEL_CLEAR = false;
        }

        // Start is called before the first frame update
        private void Start()
        {
            squadManager = AIManager.SquadManager;
            player = squadManager.player;
            difficultyMultiplier = useScalableDifficulty? SessionManager.currentDifficultyMultiplier : difficultyMultiplier;
        }

        // Update is called once per frame
        void Update()
        {
            if(!spawningEnemies && squadManager.AllDead)
            {
                if(currentWave < Waves.Length)
                {
                    spawningEnemies = true;
                    EnemySpawner.SPAWNING_ENEMIES = true;
                    StartCoroutine(SpawnWave(currentWave));
                    ++currentWave;
                }
                else
                {
                    if(!EnemySpawner.LEVEL_CLEAR)
                    {
                        EnemySpawner.LEVEL_CLEAR = true;
                        if(OnLevelClear != null)
                        {
                            OnLevelClear();
                        }
                    }
                   
                }

            }
        }

        IEnumerator SpawnWave(int number)
        {
            EnemyWave wave = Waves[number];
            List<AIController> spawnQueue = wave.GenerateWave(difficultyMultiplier);

            yield return new WaitForSeconds(wave.initialDelay);

            int spawnpointCount = 0;
            int count = 0;

            foreach (AIController ai in spawnQueue)
            {
                int tries = 0;
                while(Vector3.Distance(wave.spawnPoints[spawnpointCount].position, player.transform.position) < spawnSafeDistance && tries < 10)
                {
                    ++tries;
                    ++spawnpointCount;
                    yield return null;
                    if (spawnpointCount >= wave.spawnPoints.Length)
                        spawnpointCount = 0;
                }

                Transform spawn = wave.spawnPoints[spawnpointCount];
                squadManager.SpawnNewAI(ai, spawn.position, spawn.rotation);

                yield return new WaitForSeconds(wave.timeBetweenSpawns);

                while(squadManager.aliveCount >= wave.breakEvery && wave.breakEvery > 0)
                {
                    Debug.Log("BREAK");
                    yield return new WaitForSeconds(3);
                }

                ++count;
                ++spawnpointCount;
                if (spawnpointCount >= wave.spawnPoints.Length)
                    spawnpointCount = 0;
            }

            EnemySpawner.SPAWNING_ENEMIES = false;
            spawningEnemies = false;
        }
    }

    [System.Serializable]
    public struct EnemyWave
    {
        public float timeBetweenSpawns;
        public int initialDelay;
        public int breakEvery;
        //public int breakLength;
        public Transform[] spawnPoints;

        public AIController[] mandatoryEnemies;
        public AIController[] randomEnemies;
        public int baseRandomEnemies;

        public List<AIController> GenerateWave(float difficultyMultiplier, float mandatoryMultiplier = 1)
        {
            List<AIController> wave = new List<AIController>();

            //Add the mandatory enemies;
            int m = Mathf.RoundToInt(mandatoryEnemies.Length * mandatoryMultiplier);
            int l = mandatoryEnemies.Length;

            for (int i = 0; i < m; ++i)
            {
                int c = i % l;

                wave.Add(mandatoryEnemies[c]);
            }

            //Add the randoms
            int r = Mathf.RoundToInt(baseRandomEnemies * difficultyMultiplier);

            for (int i = 0; i < r; ++i)
            {
                int c = Random.Range(0, randomEnemies.Length);

                wave.Add(randomEnemies[c]);
            }

            return wave;
        }
    }
}

