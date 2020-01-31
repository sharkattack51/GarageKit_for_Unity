using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace GarageKit
{
    [Serializable]
    public class RemotePrefsData
    {
        /* json example
        {
            "command": "set",
            "key": "prefs_key",
            "value_type": "int",
            "int_value": 123
        }
        */

        public string command; // "set", "delete", "delete_all"
        public string key;
        public string value_type; // "int", "float", "string"

        public int int_value;
        public float float_value;
        public string string_value;
    }

    [RequireComponent(typeof(UDPReceiver))]
    public class RemotePrefs : MonoBehaviour
    {
        public Action<RemotePrefsData> WillUpdatedPrefs;
        public Action<RemotePrefsData> OnUpdatedPrefs;

        private UDPReceiver udpReceiver;


        void Awake()
        {

        }

        void Start()
        {
            udpReceiver = this.gameObject.GetComponent<UDPReceiver>();
            udpReceiver.OnReceived = OnJsonReceived;

            Debug.LogWarning(string.Format("RemotePrefs :: receive json port is [:{0}].", udpReceiver.port)
                + " send json like this {\"command\": \"set\", \"key\": \"SAMPLE\", \"value_type\": \"int\", \"int_value\": 123}");
        }

        void Update()
        {
            
        }

        void OnDisable()
        {
            udpReceiver.OnReceived = null;
            udpReceiver.Close();
            udpReceiver = null;
        }


        private void OnJsonReceived(string json)
        {
            RemotePrefsData data = null;
            
            try
            {
                data = JsonUtility.FromJson<RemotePrefsData>(json);
            }
            catch(Exception err)
            {
                Debug.LogError(err.Message);
            }

            if(data != null)
            {
                if(WillUpdatedPrefs != null)
                    WillUpdatedPrefs(data);
                
                if(data.command == "set")
                {
                    if(data.value_type == "int")
                        PlayerPrefs.SetInt(data.key, data.int_value);
                    else if(data.value_type == "float")
                        PlayerPrefs.SetFloat(data.key, data.float_value);
                    else if(data.value_type == "string")
                        PlayerPrefs.SetString(data.key, data.string_value);
                }
                else if(data.command == "delete")
                    PlayerPrefs.DeleteKey(data.key);
                else if(data.command == "delete_all")
                    PlayerPrefs.DeleteAll();
                PlayerPrefs.Save();

                if(OnUpdatedPrefs != null)
                    OnUpdatedPrefs(data);
            }
        }
    }
}
