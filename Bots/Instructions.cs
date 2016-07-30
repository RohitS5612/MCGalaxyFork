﻿/*
    Copyright 2010 MCSharp team (Modified for use with MCZall/MCLawl/MCGalaxy)
    
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

namespace MCGalaxy.Bots {
    public static class Instructions {
        
        public static Dictionary<string, Func<PlayerBot, bool>> Defined =
            new Dictionary<string, Func<PlayerBot, bool>>{
            { "walk", DoWalk }, { "teleport", DoTeleport }, { "wait", DoWait },
            { "nod", DoNod }, { "spin", DoSpin }, { "speed", DoSpeed },
            { "jump", DoJump }, { "reset", DoReset }, { "remove", DoRemove },
            { "linkscript", DoLinkscript },
        };
        
        static bool DoWalk(PlayerBot bot) {
            bot.foundPos[0] = bot.Waypoints[bot.cur].x;
            bot.foundPos[1] = bot.Waypoints[bot.cur].y;
            bot.foundPos[2] = bot.Waypoints[bot.cur].z;
            bot.movement = true;

            if ((ushort)(bot.pos[0] / 32) == (ushort)(bot.Waypoints[bot.cur].x / 32)) {
                if ((ushort)(bot.pos[2] / 32) == (ushort)(bot.Waypoints[bot.cur].z / 32)) {
                    bot.rot[0] = bot.Waypoints[bot.cur].rotx;
                    bot.rot[1] = bot.Waypoints[bot.cur].roty;
                    bot.movement = false;
                    bot.NextInstruction(); return false;
                }
            }
            bot.AdvanceRotation();
            return true;
        }
        
        static bool DoTeleport(PlayerBot bot) {
            bot.pos[0] = bot.Waypoints[bot.cur].x;
            bot.pos[1] = bot.Waypoints[bot.cur].y;
            bot.pos[2] = bot.Waypoints[bot.cur].z;
            bot.rot[0] = bot.Waypoints[bot.cur].rotx;
            bot.rot[1] = bot.Waypoints[bot.cur].roty;
            bot.NextInstruction();
            return true;
        }
        
        static bool DoWait(PlayerBot bot) {
            if (bot.countdown == 0) {
                bot.countdown = bot.Waypoints[bot.cur].seconds;
                return true;
            }
            
            bot.countdown--;
            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        static bool DoNod(PlayerBot bot) {
            if (bot.countdown == 0) {
                bot.countdown = bot.Waypoints[bot.cur].seconds;
                return true;
            }
            bot.countdown--;

            byte speed = (byte)bot.Waypoints[bot.cur].rotspeed;
            if (bot.nodUp) {
                if (bot.rot[1] > 32 && bot.rot[1] < 128) {
                    bot.nodUp = !bot.nodUp;
                } else {
                    if (bot.rot[1] + speed > 255) bot.rot[1] = 0;
                    else bot.rot[1] += speed;
                }
            } else {
                if (bot.rot[1] > 128 && bot.rot[1] < 224) {
                    bot.nodUp = !bot.nodUp;
                } else {
                    if (bot.rot[1] - speed < 0) bot.rot[1] = 255;
                    else bot.rot[1] -= speed;
                }
            }

            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        static bool DoSpin(PlayerBot bot) {
            if (bot.countdown == 0) {
                bot.countdown = bot.Waypoints[bot.cur].seconds;
                return true;
            }
            bot.countdown--;

            byte speed = (byte)bot.Waypoints[bot.cur].rotspeed;
            if (bot.rot[0] + speed > 255) bot.rot[0] = 0;
            else if (bot.rot[0] + speed < 0) bot.rot[0] = 255;
            else bot.rot[0] += speed;

            if (bot.countdown == 0) { bot.NextInstruction(); return false; }
            return true;
        }
        
        static bool DoSpeed(PlayerBot bot) {
            bot.movementSpeed = (int)Math.Round(24m / 100m * bot.Waypoints[bot.cur].seconds);
            if (bot.movementSpeed == 0) bot.movementSpeed = 1;
            bot.NextInstruction(); return false;
        }
        
        static bool DoJump(PlayerBot bot) {
            bot.jumpTimer.Elapsed += delegate {
                bot.currentjump++;
                switch (bot.currentjump) {
                    case 1:
                    case 2: bot.pos[1] += 24; break;
                    case 3: break;
                    case 4: bot.pos[1] -= 24; break;
                    case 5: bot.pos[1] -= 24; bot.jumping = false; bot.currentjump = 0; bot.jumpTimer.Stop(); break;
                }
            };
            
            bot.jumpTimer.Start();
            bot.NextInstruction();
            return false;
        }
        
        static bool DoReset(PlayerBot bot) {
            bot.cur = 0;
            return true;
        }
        
        static bool DoRemove(PlayerBot bot) {
            PlayerBot.Remove(bot);
            return true;
        }
        
        static bool DoLinkscript(PlayerBot bot) {
            if (File.Exists("bots/" + bot.Waypoints[bot.cur].newscript)) {
                string args = bot.name + " " + bot.Waypoints[bot.cur].newscript;
                Command.all.Find("botset").Use(null, args);
                return true;
            }
            bot.NextInstruction(); return true;
        }
        
        
        internal static void DoKill(PlayerBot bot) {
            ushort x = (ushort)Math.Round((decimal)bot.pos[0] / 32);
            ushort y = (ushort)((bot.pos[1] - 33) / 32);
            ushort z = (ushort)Math.Round((decimal)bot.pos[2] / 32);
            Player[] players = PlayerInfo.Online.Items;
            
            foreach (Player p in players) {
                if ((ushort)(p.pos[0] / 32) == x
                    && Math.Abs((ushort)(p.pos[1] / 32) - y) < 2
                    && (ushort)(p.pos[2] / 32) == z) {
                    p.HandleDeath(Block.Zero);
                }
            }
        }
        
        internal static void DoHunt(PlayerBot bot) {
            int dist = 75 * 32;
            Player[] players = PlayerInfo.Online.Items;
            foreach (Player p in players) {
                if (p.level != bot.level || p.invincible) continue;
                int curDist = Math.Abs(p.pos[0] - bot.pos[0]) + Math.Abs(p.pos[1] - bot.pos[1]) + Math.Abs(p.pos[2] - bot.pos[2]);
                if (curDist >= dist) continue;
                
                dist = curDist;
                bot.foundPos = p.pos;
                bot.foundRot = p.rot;
                bot.movement = true;
                
                bot.rot[1] = (byte)(255 - bot.foundRot[1]);
                if (bot.foundRot[0] < 128)
                    bot.rot[0] = (byte)(bot.foundRot[0] + 128);
                else
                    bot.rot[0] = (byte)(bot.foundRot[0] - 128);
            }
        }
    }
}
