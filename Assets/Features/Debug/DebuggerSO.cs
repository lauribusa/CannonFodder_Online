using UnityEngine;

namespace Assets.Features
{
    [CreateAssetMenu(menuName = "Debug")]
    public class DebuggerSO: ScriptableObject
    {
        public void PrintInfo(string info)
        {
            Print(info, DebugLevel.INFO);
        }

        public void PrintWarning(string warning)
        {
            Print(warning, DebugLevel.WARNING);
        }

        public void PrintError(string error) 
        { 
            Print(error, DebugLevel.ERROR);
        }
        private void Print(string message, DebugLevel debugLevel)
        {
            switch (debugLevel)
            {
                case DebugLevel.INFO:
                    Debug.Log(message);
                    break;
                case DebugLevel.WARNING:
                    Debug.LogWarning(message);
                    break;
                case DebugLevel.ERROR:
                    Debug.LogError(message);
                    break;
                default:
                    Debug.Log(message);
                    break;
            }
        }
    }

    public enum DebugLevel
    {
        INFO,
        WARNING,
        ERROR,
    }
}
