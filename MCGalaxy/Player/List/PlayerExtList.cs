﻿/*
    Copyright 2015 MCGalaxy
    
    Dual-licensed under the Educational Community License, Version 2.0 and
    the GNU General Public License, Version 3 (the "Licenses"); you may
    not use this file except in compliance with the Licenses. You may
    obtain a copy of the Licenses at
    
    http://www.opensource.org/licenses/ecl2.php
    http://www.gnu.org/licenses/gpl-3.0.html
    
    Unless required by applicable law or agreed to in writing,
    software distributed under the Licenses are distributed on an "AS IS"
    BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express
    or implied. See the Licenses for the specific language governing
    permissions and limitations under the Licenses.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MCGalaxy {
    public sealed class PlayerExtList {   
        
        char separator = ' ';
        string path;
        List<string> names = new List<string>();
        public List<string> lines = new List<string>();
        readonly object locker = new object(), saveLocker = new object();
        
        public void Add(string name, string data) {
            lock (locker) {
                names.Add(name); lines.Add(name + separator + data);
            }
        }
        
        public bool Remove(string name) {
            lock (locker) {
        		int idx = names.CaselessIndexOf(name);
                if (idx == -1) return false;
                
                names.RemoveAt(idx);
                lines.RemoveAt(idx);
                return true;
            }
        }
        
        public void AddOrReplace(string name, string data) {
            lock (locker) {
            	int idx = names.CaselessIndexOf(name);
                if (idx == -1) {
                    names.Add(name); lines.Add(name + separator + data);
                } else {
                    lines[idx] = name + separator + data;
                }
            }
        }
        
        public string Find(string name) {
            lock (locker) {
                int idx = names.CaselessIndexOf(name);
                return idx == -1 ? null : lines[idx];
            }
        }
        
        public int Count { get { lock (locker) return names.Count; } }
        
        
        public void Save() { Save(true); }
        public void Save(bool console) {
            lock (saveLocker) {
                using (StreamWriter w = new StreamWriter(path))
                    SaveEntries(w);
            }
            if (console) Server.s.Log("SAVED: " + path, true);
        }
        
        void SaveEntries(StreamWriter w) {
            lock (locker) {
                foreach (string l in lines)
                    w.WriteLine(l);
            }
        }
        
        public static PlayerExtList Load(string path, char separator = ' ') {
            PlayerExtList list = new PlayerExtList();
            list.path = path;
            list.separator = separator;
            
            if (!File.Exists(path)) {
                File.Create(path).Close();
                Server.s.Log("CREATED NEW: " + path);
                return list;
            }
            
            using (StreamReader r = new StreamReader(path, Encoding.UTF8)) {
                string line = null;
                while ((line = r.ReadLine()) != null) {
                    list.lines.Add(line);
                    int sepIndex = line.IndexOf(separator);
                    string name = sepIndex >= 0 ? line.Substring(0, sepIndex) : line;
                    list.names.Add(name);
                }
            }
            return list;
        }
    }
}