using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace MehCommandDefinitions
{
    /// <summary>Hookup class where yarn Commands are routed to their appropriate Unity targets</summary>
    public class MehCommandLookups
    {
        //Cache:
        private MehGuppy guppy;
        private MehBackground bg;
        private MehAudio audio;
        private MehDialogueMachine dialogue;
        private SwitchBreaker sb;
        private MehRoomManager room;
        private HadfieldBanner hadfield;

        private Dictionary<string, CommandFunction> cmdFuncDict;
        private Dictionary<string, CommandCoroutine> cmdCODict;

        private static string[] w; //Contains a list of words in the current command
        public static string currentCommand { get { return string.Join(" ", w); } }

        private delegate void CommandFunction(string[] array);

        private delegate Coroutine CommandCoroutine(string[] array);

        // Constructor
        public MehCommandLookups(
            MehGuppy guppy,
            MehBackground background,
            MehAudio audio,
            MehDialogueMachine dialogueMachine,
            SwitchBreaker switchBreaker,
            MehRoomManager room,
            HadfieldBanner hadfield
        )
        {
            w = new string[0];

            this.guppy = guppy;
            this.bg = background;
            this.audio = audio;
            this.dialogue = dialogueMachine;
            this.sb = switchBreaker;
            this.room = room;
            this.hadfield = hadfield;

            // populate delegate dictionary - Michel
            cmdFuncDict = new Dictionary<string, CommandFunction>();
            cmdFuncDict.Add("sbopen", SBOpen);
            cmdFuncDict.Add("sbclose", SBClose);
            cmdFuncDict.Add("sbgate", SBGate);
            cmdFuncDict.Add("setbg", SetBG);
            cmdFuncDict.Add("setface", SetFace);
            cmdFuncDict.Add("setnpcface", SetNPCFace);
            cmdFuncDict.Add("setspeaker", SetSpeaker);
            cmdFuncDict.Add("setnpc", SetNPC);
            cmdFuncDict.Add("shakenpc", ShakeNPC);
            cmdFuncDict.Add("bumpnpc", BumpNPC);
            cmdFuncDict.Add("setguppy", SetGuppy);
            cmdFuncDict.Add("shakeguppy", ShakeGuppy);
            cmdFuncDict.Add("bumpguppy", BumpGuppy);
            cmdFuncDict.Add("setspeed", SetSpeed);
            cmdFuncDict.Add("playmusic", PlayMusic);
            cmdFuncDict.Add("playaudio", PlayAudio);
            cmdFuncDict.Add("setdance", SetDance);
            cmdFuncDict.Add("madlibs", MadLibs);
            cmdFuncDict.Add("newson", ActivateBanner);
            cmdFuncDict.Add("newsoff", DeactivateBanner);
            cmdFuncDict.Add("newsname", ChangeIntervieweeName);
            cmdFuncDict.Add("newstitle", ChangeIntervieweeTitle);
            cmdFuncDict.Add("newstopic", ChangeTopic);
            cmdFuncDict.Add("printsave", PrintSaveData);

            // coroutine dictionary 
            cmdCODict = new Dictionary<string, CommandCoroutine>();
            cmdCODict.Add("wait", Wait);
        }
        
        public IEnumerator RunCommand(Command command)
        {
            w = command.text.Split(null);
            if (w.Length == 0)
                Debug.LogError("Skipping command; no words in command");

            if (cmdFuncDict.ContainsKey(w[0]))
            {
                cmdFuncDict[w[0]](w);
            }
            else if (cmdCODict.ContainsKey(w[0]))
            {
                yield return cmdCODict[w[0]](w);
            }
            else Debug.LogError("Did not recognize command " + w[0] + " in command " + command.text);
        }

        // Wrapper functions
        void SBOpen(string[] array) { this.sb.openFreeSwitch(targetPassageNames: array.Skip(1).ToArray()); }
        void SBClose(string[] array) { this.sb.closeAllNonActiveNodes(); }
        void SBGate(string[] array) { this.sb.openGateSwitch(); }
        void SetBG(string[] array) { this.bg.SetGraphic(lookup: array[1]); }
        void SetFace(string[] array) { this.guppy.SetFace(lookup: array[1]); }
        void SetNPCFace(string[] array) { this.room.SetNPCFace(parameters: array.Skip(1).ToArray()); }
        void SetSpeaker(string[] array) { this.dialogue.SetSpeakerNPC(array[1]); }
        void SetNPC(string[] array)
        {
            string[] parameters = new string[0];
            if (array.Length <1)
            {
                Debug.LogWarning("Insufficient parameters for setnpc");
            }
            else if (array.Length > 2)
            {
                parameters = array.Skip(2).ToArray();
                this.room.SetNPC(lookup: array[1], parameters: parameters);
            }
            else if (array.Length > 1)// no parameters
            {
                this.room.SetNPC(lookup: array[1], parameters: parameters);
            }
            this.dialogue.SetSpeakerNPC(array[1]);
        }
        void ShakeNPC(string[] array)
        {
            if (array.Length > 2)       this.room.ShakeNPC(magnitude: array[1], lookup: array[2]);
            else if (array.Length>1)    this.room.ShakeNPC(magnitude: array[1]);
            else                        this.room.ShakeNPC();

        }
        void BumpNPC(string[] array)
        {
            if (array.Length > 2)       this.room.BumpNPC(magnitude: array[1], lookup: array[2]);
            else if (array.Length > 1)  this.room.BumpNPC(magnitude: array[1]);
            else                        this.room.BumpNPC();

        }
        void SetGuppy(string[] array)
        {
            //The only two parameters that won't cause provlems are either null or "none" - Michel
            if (array.Length > 1)
            {
                if (array[1] != "none") { Debug.LogWarning("Set Guppy parameter [" + array[1] + "] not recognized. Only \"none\" is accepted"); return; }

                this.room.SetGuppy(array[1]);
            }
            else this.room.SetGuppy();
        }
        void ShakeGuppy(string[] array) {
            if (array.Length > 1)       this.guppy.Shake(magnitude: array[1]);
            else                        this.guppy.Shake();
        }
        void BumpGuppy(string[] array)
        {
            if (array.Length > 1)       this.guppy.Bump(magnitude: array[1]);
            else                        this.guppy.Bump();
        }
        void SetSpeed(string[] array) { this.dialogue.setSpeed(lookup: array[1]); }
        void PlayMusic(string[] array) { audio.PlayMusic(lookup: array[1], time: array[2].asFloat()); }
        void PlayAudio(string[] array) { this.audio.PlayStinger(lookup: array[1]); }
        void SetDance(string[] array) { this.guppy.SetDance(lookup: array[1]); }
        void MadLibs(string[] array) {
            array = array.Skip(1).ToArray();
            string sequence = string.Join(" ", array);
            dialogue.madlibsText(stringSequence: sequence);
        }
        void ActivateBanner(string[] array)
        {
            hadfield.ActivateBanner(w[1]);
        }
        void DeactivateBanner(string[] array)
        {
            hadfield.DeactivateBanner();
        }
        void ChangeIntervieweeName(string[] array)
        {
            string name = string.Join(" ", array.Skip(1).ToArray());
            hadfield.ChangeName(name);
        }
        void ChangeIntervieweeTitle(string[] array)
        {
            string name = string.Join(" ", array.Skip(1).ToArray());
            hadfield.ChangeTitle(name);
        }
        void ChangeTopic(string[] array)
        {
            string name = string.Join(" ", array.Skip(1).ToArray());
            hadfield.ChangeTopic(name);
        }
        void PrintSaveData(string[] array) { MehGameManager.instance.persistent.PrintData();}
        
        // Wrapper coroutines
        Coroutine Wait(string[] array)
        {
            return this.dialogue.pauseForSeconds(duration: w[1].asFloat());
        }
        
    }

    internal static class YarnParameterConversions
    {
        public static float asFloat(this string yarnParameter)
        {
            float output;
            if (float.TryParse(yarnParameter, out output))
                return output;
            else
                throw new BadYarnParameterException("Failed to translate yarn parameter " + output + " to float in command " + MehCommandLookups.currentCommand);
        }

        public static MehDialogueMachine.DialogueMode asDialogueMode(this string yarnParameter)
        {
            switch (yarnParameter) {
                case "npc": return MehDialogueMachine.DialogueMode.Npc;
                case "player": return MehDialogueMachine.DialogueMode.Guppy;
                case "guppy": return MehDialogueMachine.DialogueMode.Guppy;
                default: throw new BadYarnParameterException("Failed to translate yarn parameter " + yarnParameter + "to Meh Dialogue Mode in command " + MehCommandLookups.currentCommand);
            }
        }

        public class BadYarnParameterException : System.Exception
        {
            public BadYarnParameterException() { }
            public BadYarnParameterException(string message) : base(message) { }
        }
    }
}