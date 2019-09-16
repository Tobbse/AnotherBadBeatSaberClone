using UnityEngine;
using AudioAnalysis.AudioAnalyzerConfigs;
using Audio.BeatMappingConfigs;

namespace Global
{
    /// <summary>
    /// Global storage for data. This singleton data container can be accessed from all scenes.
    /// </summary>
    public class GlobalStorage : ScriptableObject
    {
        private static GlobalStorage Instance;

        public AudioClip AudioClip { get; set; }
        public TrackConfig TrackConfig { get; set; }
        public string AudioPath { get; set; }
        public MappingContainer MappingContainer { get; set; }
        public string Difficulty { get; set; }

        public static GlobalStorage getInstance()
        {
            if (Instance == null)
            {
                // This should normally be initialized using ScriptableObject.CreateInstance<GlobalStorage>();
                // Hwoever, this creation is not executed on the main thread, so we can't use that functionality.
                return new GlobalStorage();
            }
            return Instance;
        }

        void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(this);
                Instance = this;
            }
            else if (Instance != this)
            {
                Destroy(this);
            }
        }

        public void destroy()
        {
            Instance = null;
        }
    }

}
