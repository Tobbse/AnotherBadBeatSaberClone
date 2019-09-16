using UnityEngine;
using Global;
using MenuScoreMenu;

namespace GameSpawnedObjects
{
    /// <summary>
    /// Behavior for the Note blocks, making sure that they despawn after a while or when they reach a certain position behind the player.
    /// </summary>
    public class TimedBlock : MonoBehaviour
    {
        public const float BLOCK_DAMAGE = 3.0f;
        public const float BLOCK_HEAL = 5.0f;

        private int lifetimeCycles = 400;

        void FixedUpdate()
        {
            if (DevSettings.OVERRIDE_BLOCK_DESPAWN) enabled = false;

            lifetimeCycles--;
            if (lifetimeCycles < 0 || gameObject.transform.position.x > 5)
            {
                MissBlock();
            }
        }

        public void MissBlock()
        {
            ScoreTracker.getInstance().miss();
            Destroy(gameObject);
        }
    }

}
