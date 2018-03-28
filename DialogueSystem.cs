using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Personagem
{
    public class DialogueSystem : MonoBehaviour
    {
        GameObject talker;
        public void Talk(GameObject talkto)
        {
            talkto = talker;
            print("oi");
            
        }

    }
}