/*
	Copyright 2011 MCGalaxy
		
	Dual-licensed under the	Educational Community License, Version 2.0 and
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
using System.IO;
using System.Linq;
namespace MCGalaxy.Commands
{
	public sealed class CmdOverseer : Command
	{
		public override string name { get { return "overseer"; } }
		public override string shortcut { get { return "os"; } }
		public override string type { get { return "commands"; } }
		public override bool museumUsable { get { return true; } }
		public override LevelPermission defaultRank { get { return LevelPermission.Builder; } }
		public CmdOverseer() { }
		public override void Use(Player p, string message)
		{
            byte test;
            if (p.group.OverseerMaps == 0)
                p.SendMessage("Your rank is set to have 0 overseer maps. Therefore, you may not use overseer.");
			if (message == "") { Help(p); return; }
			Player who = Player.Find(message.Split(' ')[0]);
			string cmd = message.Split(' ')[0].ToUpper();

			string par;
			try
			{ par = message.Split(' ')[1].ToUpper(); }
			catch
			{ par = ""; }

			string par2;
			try
			{ par2 = message.Split(' ')[2]; }
			catch
			{ par2 = ""; }

			string par3;
			try
			{ par3 = message.Split(' ')[3]; }
			catch
			{ par3 = ""; }

			if (cmd == "GO")
			{
                if ((par == "1") || (par == ""))
                {
                    string mapname = properMapName(p, false);
                    if (!Server.levels.Any(l => l.name == mapname))
                    {
                        Command.all.Find("load").Use(p, mapname);
                    }
                    Command.all.Find("goto").Use(p, mapname);
                }
                else
                {
                    if (byte.TryParse(par, out test) == false)
                    {
                        Help(p);
                        return;
                    }
                    if (test > p.group.OverseerMaps)
                    {
                        p.SendMessage("Your rank does not allow you to have more than " + p.group.OverseerMaps + ".");
                        return;
                    }

                    else
                    {
                        string mapname = p.name.ToLower() + par;
                        if (!Server.levels.Any(l => l.name == mapname))
                        {
                            Command.all.Find("load").Use(p, mapname);
                        }
                        Command.all.Find("goto").Use(p, mapname);
                    }
                }
			}
			// Set Spawn (if you are on your own map level)
			else if (cmd == "SPAWN")
			{
				if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
				{
					Command.all.Find("setspawn").Use(p, "");
				}
				else { Player.SendMessage(p, "You can only change the Spawn Point when you are on your own map."); }
			}
			else if (cmd == "WEATHER")
            {
                    if (par == "SUN")
                    {
                        if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                        {
                            Command.all.Find("env").Use(p, "l weather 0");
                            return;
                        }
                        else
                        {
                            p.SendMessage("This is not your map..");
                            return;
                        }
                    }
                    else if (par == "RAIN")
                    {
                        if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                        {
                            Command.all.Find("env").Use(p, "l weather 1");
                            return;
                        }
                        else
                        {
                            p.SendMessage("This is not your map..");
                            return;
                        }
                    }
                    else if (par == "SNOW")
                    {
                        if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                        {
                            Command.all.Find("env").Use(p, "l weather 2");
                            return;
                        }
                        else
                        {
                            p.SendMessage("This is not your map..");
                            return;
                        }
                    }
                    else if (par == "NORMAL")
                    {
                        if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                        {
                            Command.all.Find("env").Use(p, "l weather 0");
                            return;
                        }
                        else
                        {
                            p.SendMessage("This is not your map..");
                            return;
                        }
                    }
                    else
                    {
                        Player.SendMessage(p, "/overseer weather [sun/rain/snow/normal] -- Changes the weather of your map.");
                        return;
                    }
            }
            // Set Env (if you are on your own map level)
            else if (cmd == "ENV")
            {
                if (par == "FOG")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {

                        int pos = message.IndexOf("fog ");
                        string fogcolor = "";
                        if (message.Split(' ').Length > 2) fogcolor = message.Substring(pos + 4);
                        if (fogcolor.Length != 6 && fogcolor != "")
                        {
                            Player.SendMessage(p, "Your fogcolor must be a 6 digit color hex code!");
                            return;
                        }
                        if (fogcolor == "")
                        {
                            Command.all.Find("env").Use(p, "l fog " + -1);
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l fog " + fogcolor);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "CLOUDS")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("clouds ");
                        string cloudcolor = "";
                        if (message.Split(' ').Length > 2) cloudcolor = message.Substring(pos + 7);
                        if (cloudcolor.Length != 6 && cloudcolor != "")
                        {
                            Player.SendMessage(p, "Your cloudcolor must be a 6 digit color hex code!");
                            return;
                        }
                        if (cloudcolor == "")
                        {
                            Command.all.Find("env").Use(p, "l clouds " + -1);
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l clouds " + cloudcolor);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "SKY")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("sky ");
                        string skycolor = "";
                        if (message.Split(' ').Length > 2) skycolor = message.Substring(pos + 4);
                        if (skycolor.Length != 6 && skycolor != "")
                        {
                            Player.SendMessage(p, "Your skycolor must be a 6 digit color hex code!");
                            return;
                        }
                        if (skycolor == "")
                        {
                            Command.all.Find("env").Use(p, "l sky " + -1);
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l sky " + skycolor);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "SHADOW")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("shadow ");
                        string shadowcolor = "";
                        if (message.Split(' ').Length > 2) shadowcolor = message.Substring(pos + 7);
                        if (shadowcolor.Length != 6 && shadowcolor != "")
                        {
                            Player.SendMessage(p, "Your shadowcolor must be a 6 digit color hex code!");
                            return;
                        }
                        if (shadowcolor == "")
                        {
                            Command.all.Find("env").Use(p, "l shadow " + -1);
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l shadow " + shadowcolor);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "SUNLIGHT")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("sunlight ");
                        string sunlightcolor = "";
                        if (message.Split(' ').Length > 2) sunlightcolor = message.Substring(pos + 9);
                        if (sunlightcolor.Length != 6 && sunlightcolor != "")
                        {
                            Player.SendMessage(p, "Your sunlightcolor must be a 6 digit color hex code!");
                            return;
                        }
                        if (sunlightcolor == "")
                        {
                            Command.all.Find("env").Use(p, "l sunlight " + -1);
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l sunlight " + sunlightcolor);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "WATERLEVEL")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("waterlevel ");
                        string waterlevel = "";
                        if (message.Split(' ').Length > 2) waterlevel = message.Substring(pos + 11);
                        if (waterlevel == "")
                        {
                            Command.all.Find("env").Use(p, "l level reset");
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l level " + waterlevel);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "HORIZON")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("horizon ");
                        string horizonblock = "";
                        if (message.Split(' ').Length > 2) horizonblock = message.Substring(pos + 8);
                        if (horizonblock == "")
                        {
                            Command.all.Find("env").Use(p, "l water reset");
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l water " + horizonblock);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else if (par == "BEDROCK")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("bedrock ");
                        string bedrockblock = "";
                        if (message.Split(' ').Length > 2) bedrockblock = message.Substring(pos + 8);
                        if (bedrockblock == "")
                        {
                            Command.all.Find("env").Use(p, "l bedrock reset");
                        }
                        else
                        {
                            Command.all.Find("env").Use(p, "l bedrock " + bedrockblock);
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                else
                {
                    Player.SendMessage(p, "/overseer env [fog/clouds/sky/shadow/sunlight] [hex color code] -- Changes cloud color of your map");
                    Player.SendMessage(p, "/overseer env waterlevel -- Sets the water height of your map");
                    Player.SendMessage(p, "/overseer env horizon -- Sets what block the \"ocean\" shows outside your map");
                    Player.SendMessage(p, "/overseer env bedrock -- Sets what block replaces the bedrock below sea level in your map");
                    Player.SendMessage(p, "  Warning: Air,Shrub,Glass,Flowers,Mushroom,Rope,Fire cannot be used for horizon/bedrock env!");
                    Player.SendMessage(p, "  Note: If no hex or block is given, the default will be used.");
                }
            }
			// Map Commands
			else if (cmd == "MAP")
			{
				if (par == "ADD")
				{
					if ((File.Exists("levels/" + p.name.ToLower() + ".lvl")) || (File.Exists("levels/" + p.name.ToLower() + "00.lvl")))
					{
                        foreach(string filenames in Directory.GetFiles("levels"))
                        {
                            for(int i = 1; i < p.group.OverseerMaps + 2; i++)
                            {
                                //Not the best way to do it, but I'm lazy
                                if (i == 1)
                                    i = 2;
                                if(i != 0)
                                {
                                if(!File.Exists("levels/" + p.name.ToLower() + i + ".lvl"))
                                {
                                    if(i > p.group.OverseerMaps)
                                    {
                                        p.SendMessage("You have reached the limit for your overseer maps.."); return;
                                    }
                                    Player.SendMessage(p, Server.DefaultColor + "Creating a new map for you.." + p.name.ToLower() + i.ToString());
                                    string mType;
                                    if (par2.ToUpper() == "" || par2.ToUpper() == "DESERT" || par2.ToUpper() == "FLAT" || par2.ToUpper() == "FOREST" || par2.ToUpper() == "ISLAND" || par2.ToUpper() == "MOUNTAINS" || par2.ToUpper() == "OCEAN" || par2.ToUpper() == "PIXEL" || par2.ToUpper() == "SPACE")
                                    {
                                        if (par2 != "")
                                        {
                                            mType = par2;
                                        }
                                        else
                                        {
                                            mType = "flat";
                                        }
                                        Command.all.Find("newlvl").Use(p, p.name.ToLower() + i.ToString() + " " + mSize(p) + " " + mType);
                                    }
                                    else
                                    {
                                        Player.SendMessage(p, "A wrong map type was specified. Valid map types: Desert, flat, forest, island, mountians, ocean, pixel and space.");
                                    }
                                    return;
                                }
                                }
                            }
                        }
					}
					else
					{
						string mType;
						if (par2.ToUpper() == "" || par2.ToUpper() == "DESERT" || par2.ToUpper() == "FLAT" || par2.ToUpper() == "FOREST" || par2.ToUpper() == "ISLAND" || par2.ToUpper() == "MOUNTAINS" || par2.ToUpper() == "OCEAN" || par2.ToUpper() == "PIXEL" || par2.ToUpper() == "SPACE")
						{
							if (par2 != "")
							{
								mType = par2;
							}
							else
							{
								mType = "flat";
							}
							Player.SendMessage(p, "Creating your map, " + p.color + p.name);
							Command.all.Find("newlvl").Use(p, p.name.ToLower() + " " + mSize(p) + " " + mType);
						}
						else
						{
							Player.SendMessage(p, "A wrong map type was specified. Valid map types: Desert, flat, forest, island, mountians, ocean, pixel and space.");
						}
					}

				}
				else if (par == "PHYSICS")
				{
					if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
					{
						if (par2 != "")
						{
							if (par2 == "0")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 0");
							}
							else if (par2 == "1")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 1");
							}
							else if (par2 == "2")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 2");
							}
							else if (par2 == "3")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 3");
							}
							else if (par2 == "4")
							{
								Command.all.Find("physics").Use(p, p.level.name + " 4");
							}
                            else if (par2 == "5")
                            {
                                Command.all.Find("physics").Use(p, p.level.name + " 5");
                            }
						}
						else { Player.SendMessage(p, "You didn't enter a number! Please enter one of these numbers: 0, 1, 2, 3, 4, 5"); }
					}
					else { Player.SendMessage(p, "You have to be on one of your maps to set its physics!"); }
				}
				// Delete your map
				else if (par == "DELETE")
				{
					if (par2 == "")
					{
						Player.SendMessage(p, "To delete one of your maps type /os map delete <map number>");
					}
					else if (par2 == "1")
					{
						Command.all.Find("deletelvl").Use(p, properMapName(p, false));
						Player.SendMessage(p, "Your 1st map has been removed.");
						return;
					}
					else if (byte.TryParse(par2, out test) == true)
					{
						Command.all.Find("deletelvl").Use(p, p.name.ToLower() + par2);
						Player.SendMessage(p, "Your map has been removed.");
						return;
					}
                    			else
                    			{
                        			Help(p);
                        			return;
                    			}

				}
				//Save your map
				else if (par == "SAVE")
                		{
                    			if (par2 == "")
                    			{
                        			Player.SendMessage(p, "To save one of your maps type /os map save <map number>");
                    			}
                    			else if (par2 == "1")
                    			{
                        			Command.all.Find("save").Use(p, properMapName(p, false));
                        			Player.SendMessage(p, "Your 1st map has been saved.");
                        			return;
                    			}
                    			else if (byte.TryParse(par2, out test) == true)
                    			{
                        			Command.all.Find("save").Use(p, p.name.ToLower() + par2);
                        			Player.SendMessage(p, "Your map has been saved.");
                        			return;
                    			}
                    			else
                    			{
                 				Help(p);
                        			return;
                    			}
                		}
                //Give your map a custom motd
                else if (par == "MOTD")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        int pos = message.IndexOf("motd ");
                        string newMotd = "";
                        if (message.Split(' ').Length > 2) newMotd = message.Substring(pos + 5);

                        if (newMotd == "")
                        {
                            Command.all.Find("map").Use(p, "motd ignore");
                            p.level.Save();
                            Level.SaveSettings(p.level);
                            return;
                        }
                        else if (newMotd.Length > 30)
                        {
                            Player.SendMessage(p, "Your motd can be no longer than %b30" + Server.DefaultColor + " characters.");
                            return;
                        }
                        else
                        {
                            Command.all.Find("map").Use(p, "motd " + newMotd);
                            p.level.Save();
                            Level.SaveSettings(p.level);
                            return;
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                //Change the gun permission of your map
                else if (par == "GUNS")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        Command.all.Find("allowguns").Use(p, null);
                        return;
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                //Change the pervisit of your map
                else if (par == "PERVISIT")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        if (par2 == "")
                        {
                            Command.all.Find("pervisit").Use(p, Server.defaultRank);
                            return;
                        }
                        else
                        {
                            Command.all.Find("pervisit").Use(p, par2);
                            return;
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
                //Give your map a texture
                else if (par == "TEXTURE")
                {
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                    {
                        if (par2 == "")
                        {
                            Player.SendMessage(p, "Removing current texture.");
                            Command.all.Find("texture").Use(p, "level http://null.png");
                            return;
                        }
                        if (par2.Substring(par2.Length - 4) != ".png")
                        {
                            Player.SendMessage(p, "Your texture image must end in .png");
                            return;
                        }
                        else
                        {
                            Command.all.Find("texture").Use(p, "level " + par2);
                            Player.SendMessage(p, "Your texture has been updated!");
                            return;
                        }
                    }
                    else
                    {
                        p.SendMessage("This is not your map..");
                        return;
                    }
                }
				else
				{
					//all is good here :)
					Player.SendMessage(p, "/overseer map add [type - default is flat] -- Creates your map");
					Player.SendMessage(p, "/overseer map physics -- Sets the physics on your map.");
					Player.SendMessage(p, "/overseer map delete -- Deletes your map");
					Player.SendMessage(p, "/overseer map save -- Saves your map");
					Player.SendMessage(p, "/overseer map motd -- Changes the motd of your map");
                    Player.SendMessage(p, "/overseer map guns -- Toggles if guns can be used on your map");
                    Player.SendMessage(p, "/overseer map pervisit %b[default is " + Server.defaultRank + "]" + Server.DefaultColor + " -- Changes the pervisit of you map");
                    Player.SendMessage(p, "/overseer map texture -- Add a texture to your map");
                    Player.SendMessage(p, "  Textures: If your URL is too long, use the \"<\" symbol to continue it on another line.");
					Player.SendMessage(p, "  Map Types: Desert, flat, forest, island, mountians, ocean, pixel and space");
					Player.SendMessage(p, "  Motd: If no message is provided, the default message will be used.");
				}
			}
			else if (cmd == "ZONE")
			{
				// List zones on a single block(dont need to touch this :) )
				if (par == "LIST")
				{
					Command zone = Command.all.Find("zone");
					zone.Use(p, "");

				}
				// Add Zone to your personal map(took a while to get it to work(it was a big derp))
				else if (par == "ADD")
				{
					if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
					{
						if (par2 != "")
						{
							Command.all.Find("ozone").Use(p, par2);
							Player.SendMessage(p, par2 + " has been allowed building on your map.");
						}
						else
						{
							Player.SendMessage(p, "You did not specify a name to allow building on your map.");
						}
					}
					else { Player.SendMessage(p, "You must be on one of your maps to add or delete zones"); }
				}
				else if (par == "DEL")
				{
                    if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
					{
						// I need to add the ability to delete a single zone, I need help!
						if ((par2.ToUpper() == "ALL") || (par2.ToUpper() == ""))
						{
							Command zone = Command.all.Find("zone");
							Command click = Command.all.Find("click");
							zone.Use(p, "del all");
							click.Use(p, 0 + " " + 0 + " " + 0);
						}
					}
					else { Player.SendMessage(p, "You have to be on one of your maps to delete or add zones!"); }
				}
				else
				{
					// Unknown Zone Request
					Player.SendMessage(p, "/overseer ZONE add [playername or rank] -- Add a zone for a player or a rank."); ;
					Player.SendMessage(p, "/overseer ZONE del [all] -- Deletes all zones.");
					Player.SendMessage(p, "/overseer ZONE list -- show active zones on brick.");
					Player.SendMessage(p, "You can only delete all zones for now.");
				}
			}
			//Lets player load the level
			else if (cmd == "LOAD")
			{
				if (par != "")
				{
					if (par == "1")
					{
						Command.all.Find("load").Use(p, properMapName(p, false));
						Player.SendMessage(p, "Your level is now loaded.");
					}
					else if (byte.TryParse(par, out test) == true)
					{
						Command.all.Find("load").Use(p, p.name + par);
						Player.SendMessage(p, "Your level is now loaded.");
					}
                    else
                    {
                        Help(p);
                        return;
                    }
				}
				else { Player.SendMessage(p, "Type /os load <number> to load one of your maps"); }
			}
			//Kicks all other players from your current map
			else if (cmd == "KICKALL")
			{
                if (p.level.name.ToUpper().StartsWith(p.name.ToUpper()))
                {
                    Player.players.ForEach(delegate(Player pl) { if (pl.level == p.level && pl.name != p.name) Command.all.Find("goto").Use(pl, Server.mainLevel.name); });
                }
                else 
                { 
                    p.SendMessage("This is not your map..");
                }
			}
			else
			{
				Help(p);
			}
		}

		public override void Help(Player p)
		{
			// Remember to include or exclude the spoof command(s) -- MakeMeOp
			Player.SendMessage(p, "/overseer [command string] - sends command to The Overseer");
			Player.SendMessage(p, "Accepted Commands:");
			Player.SendMessage(p, "Go, map, spawn, zone, load, kickall, env, weather");
			Player.SendMessage(p, "/os - Command shortcut.");
		}

		public string properMapName(Player p, bool Ext)
		{
			/* Returns the proper name of the User Level. By default the User Level will be named
			 * "UserName" but was earlier named "UserName00". Therefore the Script checks if the old
			 * map name exists before trying the new (and correct) name. All Operations will work with
			 * both map names (UserName and UserName00)
			 * I need to figure out how to add a system to do this with the players second map.
			 */
			string r = "";
			if (File.Exists(Directory.GetCurrentDirectory() + "\\levels\\" + p.name.ToLower() + "00.lvl"))
			{
				r = p.name.ToLower() + "00";
			}
			else
			{
				r = p.name.ToLower();
			}
			if (Ext == true) { r = r + ".lvl"; }
			return r;
		}

		public string mSize(Player p)
		{

			return "128 64 128";
		}
	}
}
