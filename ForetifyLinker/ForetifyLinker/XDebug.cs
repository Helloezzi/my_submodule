using System;

namespace ForetifyLinker
{
    class XDebug : IDebug
    {
        public string Message { get; private set; }

        public void Write(string msg, object[] obj = null)
        {
            Message = msg + " ";

            if (obj != null)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    Message += obj.ToString();
                }                
            }

#if UNITY_EDITOR
            UnityEngine.Debug.Log(Message);
#else
            Console.WriteLine(Message);
#endif
        }
    }
}
