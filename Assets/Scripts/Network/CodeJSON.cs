using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using UnityEngine;


namespace Game.Network {
    internal class CodeJSON : IServerData {
        public bool worked { get; set; }
        public int trys { get; set; }
        public string gen_code { get; set; }
        public string host_ip { get; set; }
        public int team { get; set; }
    }
}