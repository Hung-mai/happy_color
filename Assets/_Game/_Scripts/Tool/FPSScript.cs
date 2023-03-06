
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace FBCapture {
    public class FPSScript : MonoBehaviour {
        /// <summary>
        /// Delta time
        /// </summary>
        float deltaTime = 0.0f;

        /// <summary>
        /// It will be used for printing out fps text on screen
        /// </summary>
        public TextMeshProUGUI text;
        //public TextMeshProUGUI text2;

        void Start() {
        }

        void Update() {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            text.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
            //text2.text = string.Format("{0:0.000} ms ({1:0.} fps)", Time.fixedDeltaTime, 1.0f / Time.fixedDeltaTime);
        }
    }

}