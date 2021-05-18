using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Network {
    internal class ServerRequest<T> where T : IServerData{
        public string msg { get; set; }
        public string result { get; set; }
        public string error { get; set; }
        public int timestamp { get; set; }
        public Dictionary<string,T> data { get; set; }
        public string permitted_chars { get; set; }
        public string hash { get; set; }
    }
}