using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace CodeStage.AntiCheat.Examples
{
	// Token: 0x020001AB RID: 427
	[AddComponentMenu("")]
	public class ActTesterGui : MonoBehaviour
	{
		// Token: 0x06000FA8 RID: 4008 RVA: 0x0000F137 File Offset: 0x0000D337
		public void OnSpeedHackDetected()
		{
			this.speedHackDetected = true;
			Debug.Log("Speed hack Detected!");
		}

		// Token: 0x06000FA9 RID: 4009 RVA: 0x0000F14A File Offset: 0x0000D34A
		public void OnInjectionDetected()
		{
			this.injectionDetected = true;
			Debug.Log("Injection Detected!");
		}

		// Token: 0x06000FAA RID: 4010 RVA: 0x0000F15D File Offset: 0x0000D35D
		public void OnInjectionDetectedWithCause(string cause)
		{
			this.injectionDetected = true;
			Debug.Log("Injection Detected! Cause: " + cause);
		}

		// Token: 0x06000FAB RID: 4011 RVA: 0x0000F176 File Offset: 0x0000D376
		public void OnObscuredTypeCheatingDetected()
		{
			this.obscuredTypeCheatDetected = true;
			Debug.Log("Obscured Vars Cheating Detected!");
		}

		// Token: 0x06000FAC RID: 4012 RVA: 0x0000F189 File Offset: 0x0000D389
		public void OnWallHackDetected()
		{
			this.wallHackCheatDetected = true;
			Debug.Log("Wall hack Detected!");
		}

		// Token: 0x06000FAD RID: 4013 RVA: 0x0000F19C File Offset: 0x0000D39C
		private void OnValidate()
		{
			if (Application.isPlaying)
			{
				ObscuredPrefs.CryptoKey = this.prefsEncryptionKey;
			}
		}

		// Token: 0x06000FAE RID: 4014 RVA: 0x0000F1B3 File Offset: 0x0000D3B3
		private void Awake()
		{
			ObscuredPrefs.CryptoKey = this.prefsEncryptionKey;
			ObscuredPrefs.onAlterationDetected = new Action(this.SavesAlterationDetected);
			ObscuredPrefs.onPossibleForeignSavesDetected = new Action(this.ForeignSavesDetected);
		}

		// Token: 0x06000FAF RID: 4015 RVA: 0x0000F1E2 File Offset: 0x0000D3E2
		private void Start()
		{
			this.ObscuredStringExample();
			this.ObscuredIntExample();
			this.ObscuredFloatExample();
			this.ObscuredVector3Example();
			base.Invoke("RandomizeObscuredVars", UnityEngine.Random.Range(1f, 10f));
		}

		// Token: 0x06000FB0 RID: 4016 RVA: 0x0006F3A4 File Offset: 0x0006D5A4
		private void RandomizeObscuredVars()
		{
			this.obscuredInt.RandomizeCryptoKey();
			this.obscuredFloat.RandomizeCryptoKey();
			this.obscuredString.RandomizeCryptoKey();
			this.obscuredVector3.RandomizeCryptoKey();
			base.Invoke("RandomizeObscuredVars", UnityEngine.Random.Range(1f, 10f));
		}

		// Token: 0x06000FB1 RID: 4017 RVA: 0x0006F3F8 File Offset: 0x0006D5F8
		private void ObscuredStringExample()
		{
			this.logBuilder.Length = 0;
			this.logBuilder.AppendLine("[ACTk] <b>[ ObscuredString test ]</b>");
			ObscuredString.SetNewCryptoKey("I LOVE MY GIRLz");
			string text = "the Goscurry is not a lie ;)";
			this.logBuilder.AppendLine("Original string:\n" + text);
			ObscuredString obscuredString = text;
			this.logBuilder.AppendLine("How your string is stored in memory when obscured:\n" + obscuredString.GetEncrypted());
			Debug.Log(this.logBuilder);
		}

		// Token: 0x06000FB2 RID: 4018 RVA: 0x0006F478 File Offset: 0x0006D678
		private void ObscuredIntExample()
		{
			this.logBuilder.Length = 0;
			this.logBuilder.AppendLine("[ACTk] <b>[ ObscuredInt test ]</b>");
			ObscuredInt.SetNewCryptoKey(434523);
			int num = 5;
			this.logBuilder.AppendLine("Original lives count: " + num);
			ObscuredInt obscuredInt = num;
			this.logBuilder.AppendLine("How your lives count is stored in memory when obscured: " + obscuredInt.GetEncrypted());
			ObscuredInt.SetNewCryptoKey(666);
			num = obscuredInt;
			obscuredInt -= 2;
			obscuredInt = obscuredInt + num + 10;
			obscuredInt /= 2;
			obscuredInt = ++obscuredInt;
			ObscuredInt.SetNewCryptoKey(999);
			obscuredInt = ++obscuredInt;
			obscuredInt = --obscuredInt;
			this.logBuilder.AppendLine(string.Concat(new object[]
			{
				"Lives count after few usual operations: ",
				obscuredInt,
				" (",
				obscuredInt.ToString("X"),
				"h)"
			}));
			Debug.Log(this.logBuilder);
		}

		// Token: 0x06000FB3 RID: 4019 RVA: 0x0006F5A4 File Offset: 0x0006D7A4
		private void ObscuredFloatExample()
		{
			this.logBuilder.Length = 0;
			this.logBuilder.AppendLine("[ACTk] <b>[ ObscuredFloat test ]</b>");
			ObscuredFloat.SetNewCryptoKey(404);
			float num = 99.9f;
			this.logBuilder.AppendLine("Original health bar: " + num);
			ObscuredFloat obscuredFloat = num;
			this.logBuilder.AppendLine("How your health bar is stored in memory when obscured: " + obscuredFloat.GetEncrypted());
			ObscuredFloat.SetNewCryptoKey(666);
			obscuredFloat += 6f;
			obscuredFloat -= 1.5f;
			obscuredFloat = ++obscuredFloat;
			obscuredFloat = --obscuredFloat;
			obscuredFloat = --obscuredFloat;
			obscuredFloat = num - obscuredFloat + 10.5f;
			this.logBuilder.AppendLine("Health bar after few usual operations: " + obscuredFloat);
			Debug.Log(this.logBuilder);
		}

		// Token: 0x06000FB4 RID: 4020 RVA: 0x0006F6A4 File Offset: 0x0006D8A4
		private void ObscuredVector3Example()
		{
			
		}

		// Token: 0x06000FB5 RID: 4021 RVA: 0x0000F216 File Offset: 0x0000D416
		private void SavesAlterationDetected()
		{
			this.savesAlterationDetected = true;
		}

		// Token: 0x06000FB6 RID: 4022 RVA: 0x0000F21F File Offset: 0x0000D41F
		private void ForeignSavesDetected()
		{
			this.foreignSavesDetected = true;
		}

		// Token: 0x06000FB7 RID: 4023 RVA: 0x0006F788 File Offset: 0x0006D988
		private void OnGUI()
		{
			GUIStyle guistyle = new GUIStyle(GUI.skin.label);
			guistyle.alignment = TextAnchor.UpperCenter;
			GUILayout.BeginArea(new Rect(10f, 5f, (float)(Screen.width - 20), (float)(Screen.height - 10)));
			GUILayout.Label("<color=\"#0287C8\"><b>Anti-Cheat Toolkit Sandbox</b></color>", guistyle, new GUILayoutOption[0]);
			GUILayout.Label("Here you can overview common ACTk features and try to cheat something yourself.", guistyle, new GUILayoutOption[0]);
			GUILayout.Space(5f);
			this.currentTab = GUILayout.Toolbar(this.currentTab, this.tabs, new GUILayoutOption[0]);
			if (this.currentTab == 0)
			{
				GUILayout.Label("ACTk offers own collection of the secure types to let you protect your variables from <b>ANY</b> memory hacking tools (Cheat Engine, ArtMoney, GameCIH, Game Guardian, etc.).", new GUILayoutOption[0]);
				GUILayout.Space(5f);
				using (new HorizontalLayout(new GUILayoutOption[0]))
				{
					GUILayout.Label("<b>Obscured types:</b>\n<color=\"#75C4EB\">" + this.GetAllSimpleObscuredTypes() + "</color>", new GUILayoutOption[]
					{
						GUILayout.MinWidth(130f)
					});
					GUILayout.Space(10f);
					using (new VerticalLayout(GUI.skin.box))
					{
						GUILayout.Label("Below you can try to cheat few variables of the regular types and their obscured (secure) analogues (you may change initial values from Tester object inspector):", new GUILayoutOption[0]);
						GUILayout.Space(10f);
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>string:</b> " + this.regularString, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.regularString += (char)UnityEngine.Random.Range(97, 122);
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.regularString = string.Empty;
							}
						}
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>ObscuredString:</b> " + this.obscuredString, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.obscuredString += (char)UnityEngine.Random.Range(97, 122);
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.obscuredString = string.Empty;
							}
						}
						GUILayout.Space(10f);
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>int:</b> " + this.regularInt, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.regularInt += UnityEngine.Random.Range(1, 100);
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.regularInt = 0;
							}
						}
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>ObscuredInt:</b> " + this.obscuredInt, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.obscuredInt += UnityEngine.Random.Range(1, 100);
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.obscuredInt = 0;
							}
						}
						GUILayout.Space(10f);
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>float:</b> " + this.regularFloat, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.regularFloat += UnityEngine.Random.Range(1f, 100f);
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.regularFloat = 0f;
							}
						}
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>ObscuredFloat:</b> " + this.obscuredFloat, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.obscuredFloat += UnityEngine.Random.Range(1f, 100f);
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.obscuredFloat = 0f;
							}
						}
						GUILayout.Space(10f);
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>Vector3:</b> " + this.regularVector3, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.regularVector3 += UnityEngine.Random.insideUnitSphere;
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.regularVector3 = Vector3.zero;
							}
						}
						using (new HorizontalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<b>ObscuredVector3:</b> " + this.obscuredVector3, new GUILayoutOption[]
							{
								GUILayout.Width(250f)
							});
							if (GUILayout.Button("Add random value", new GUILayoutOption[0]))
							{
								this.obscuredVector3 += UnityEngine.Random.insideUnitSphere;
							}
							if (GUILayout.Button("Reset", new GUILayoutOption[0]))
							{
								this.obscuredVector3 = Vector3.zero;
							}
						}
					}
				}
			}
			else if (this.currentTab == 1)
			{
				GUILayout.Label("ACTk has secure layer for the PlayerPrefs: <color=\"#75C4EB\">ObscuredPrefs</color>. It protects data from view, detects any cheating attempts, optionally locks data to the current device and supports additional data types.", new GUILayoutOption[0]);
				GUILayout.Space(5f);
				using (new HorizontalLayout(new GUILayoutOption[0]))
				{
					GUILayout.Label("<b>Supported types:</b>\n" + this.GetAllObscuredPrefsDataTypes(), new GUILayoutOption[]
					{
						GUILayout.MinWidth(130f)
					});
					using (new VerticalLayout(GUI.skin.box))
					{
						GUILayout.Label("Below you can try to cheat both regular PlayerPrefs and secure ObscuredPrefs:", new GUILayoutOption[0]);
						using (new VerticalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<color=\"#FF4040\"><b>PlayerPrefs:</b></color>\neasy to cheat, only 3 supported types", guistyle, new GUILayoutOption[0]);
							GUILayout.Space(5f);
							if (string.IsNullOrEmpty(this.regularPrefs))
							{
								this.LoadRegularPrefs();
							}
							using (new HorizontalLayout(new GUILayoutOption[0]))
							{
								GUILayout.Label(this.regularPrefs, new GUILayoutOption[]
								{
									GUILayout.Width(270f)
								});
								using (new VerticalLayout(new GUILayoutOption[0]))
								{
									using (new HorizontalLayout(new GUILayoutOption[0]))
									{
										if (GUILayout.Button("Save", new GUILayoutOption[0]))
										{
											this.SaveRegularPrefs();
										}
										if (GUILayout.Button("Load", new GUILayoutOption[0]))
										{
											this.LoadRegularPrefs();
										}
									}
									if (GUILayout.Button("Delete", new GUILayoutOption[0]))
									{
										this.DeleteRegularPrefs();
									}
								}
							}
						}
						GUILayout.Space(5f);
						using (new VerticalLayout(new GUILayoutOption[0]))
						{
							GUILayout.Label("<color=\"#02C85F\"><b>ObscuredPrefs:</b></color>\nsecure, lot of additional types and extra options", guistyle, new GUILayoutOption[0]);
							GUILayout.Space(5f);
							if (string.IsNullOrEmpty(this.obscuredPrefs))
							{
								this.LoadObscuredPrefs();
							}
							using (new HorizontalLayout(new GUILayoutOption[0]))
							{
								GUILayout.Label(this.obscuredPrefs, new GUILayoutOption[]
								{
									GUILayout.Width(270f)
								});
								using (new VerticalLayout(new GUILayoutOption[0]))
								{
									using (new HorizontalLayout(new GUILayoutOption[0]))
									{
										if (GUILayout.Button("Save", new GUILayoutOption[0]))
										{
											this.SaveObscuredPrefs();
										}
										if (GUILayout.Button("Load", new GUILayoutOption[0]))
										{
											this.LoadObscuredPrefs();
										}
									}
									if (GUILayout.Button("Delete", new GUILayoutOption[0]))
									{
										this.DeleteObscuredPrefs();
									}
									using (new HorizontalLayout(new GUILayoutOption[0]))
									{
										GUILayout.Label("LockToDevice level", new GUILayoutOption[0]);
										this.PlaceUrlButton("http://j.mp/1gxg1tf");
									}
									this.savesLock = GUILayout.SelectionGrid(this.savesLock, new string[]
									{
										ObscuredPrefs.DeviceLockLevel.None.ToString(),
										ObscuredPrefs.DeviceLockLevel.Soft.ToString(),
										ObscuredPrefs.DeviceLockLevel.Strict.ToString()
									}, 3, new GUILayoutOption[0]);
									ObscuredPrefs.lockToDevice = (ObscuredPrefs.DeviceLockLevel)this.savesLock;
									GUILayout.Space(5f);
									using (new HorizontalLayout(new GUILayoutOption[0]))
									{
										ObscuredPrefs.preservePlayerPrefs = GUILayout.Toggle(ObscuredPrefs.preservePlayerPrefs, "preservePlayerPrefs", new GUILayoutOption[0]);
										this.PlaceUrlButton("http://j.mp/1iBK5pz");
									}
									using (new HorizontalLayout(new GUILayoutOption[0]))
									{
										ObscuredPrefs.emergencyMode = GUILayout.Toggle(ObscuredPrefs.emergencyMode, "emergencyMode", new GUILayoutOption[0]);
										this.PlaceUrlButton("http://j.mp/1FRAL5L");
									}
									using (new HorizontalLayout(new GUILayoutOption[0]))
									{
										ObscuredPrefs.readForeignSaves = GUILayout.Toggle(ObscuredPrefs.readForeignSaves, "readForeignSaves", new GUILayoutOption[0]);
										this.PlaceUrlButton("http://j.mp/1LCdpDa");
									}
									GUILayout.Space(5f);
									GUILayout.Label(string.Concat(new object[]
									{
										"<color=\"",
										(!this.savesAlterationDetected) ? "#02C85F" : "#FF4040",
										"\">Saves modification detected: ",
										this.savesAlterationDetected,
										"</color>"
									}), new GUILayoutOption[0]);
									GUILayout.Label(string.Concat(new object[]
									{
										"<color=\"",
										(!this.foreignSavesDetected) ? "#02C85F" : "#FF4040",
										"\">Foreign saves detected: ",
										this.foreignSavesDetected,
										"</color>"
									}), new GUILayoutOption[0]);
								}
							}
						}
						GUILayout.Space(5f);
						this.PlaceUrlButton("http://docs.unity3d.com/ScriptReference/PlayerPrefs.html", "Visit docs to see where PlayerPrefs are stored", -1);
					}
				}
			}
			else
			{
				GUILayout.Label("ACTk is able to detect some types of cheating to let you take action on the cheating players. This example scene has all possible detectors and all of them are automatically start on scene start.", new GUILayoutOption[0]);
				GUILayout.Space(5f);
				using (new VerticalLayout(GUI.skin.box))
				{
					GUILayout.Label("<b>Speed Hack Detector</b>", new GUILayoutOption[0]);
					GUILayout.Label("Allows to detect Cheat Engine's speed hack (and maybe some other speed hack tools) usage.", new GUILayoutOption[0]);
					GUILayout.Label(string.Concat(new string[]
					{
						"<color=\"",
						(!this.speedHackDetected) ? "#02C85F" : "#FF4040",
						"\">Detected: ",
						this.speedHackDetected.ToString().ToLower(),
						"</color>"
					}), new GUILayoutOption[0]);
					GUILayout.Space(10f);
					GUILayout.Label("<b>Obscured Cheating Detector</b>", new GUILayoutOption[0]);
					GUILayout.Label("Detects cheating of any Obscured type (except ObscuredPrefs, it has own detection features) used in project.", new GUILayoutOption[0]);
					GUILayout.Label(string.Concat(new string[]
					{
						"<color=\"",
						(!this.obscuredTypeCheatDetected) ? "#02C85F" : "#FF4040",
						"\">Detected: ",
						this.obscuredTypeCheatDetected.ToString().ToLower(),
						"</color>"
					}), new GUILayoutOption[0]);
					GUILayout.Space(10f);
					GUILayout.Label("<b>WallHack Detector</b>", new GUILayoutOption[0]);
					GUILayout.Label("Detects common types of wall hack cheating: walking through the walls (Rigidbody and CharacterController modules), shooting through the walls (Raycast module), looking through the walls (Wireframe module).", new GUILayoutOption[0]);
					GUILayout.Label(string.Concat(new string[]
					{
						"<color=\"",
						(!this.wallHackCheatDetected) ? "#02C85F" : "#FF4040",
						"\">Detected: ",
						this.wallHackCheatDetected.ToString().ToLower(),
						"</color>"
					}), new GUILayoutOption[0]);
					GUILayout.Space(10f);
					GUILayout.Label("<b>Injection Detector</b>", new GUILayoutOption[0]);
					GUILayout.Label("Allows to detect foreign managed assemblies in your application.", new GUILayoutOption[0]);
					GUILayout.Label(string.Concat(new string[]
					{
						"<color=\"",
						(!this.injectionDetected) ? "#02C85F" : "#FF4040",
						"\">Detected: ",
						this.injectionDetected.ToString().ToLower(),
						"</color>"
					}), new GUILayoutOption[0]);
				}
			}
			GUILayout.EndArea();
		}

		// Token: 0x06000FB8 RID: 4024 RVA: 0x000707F0 File Offset: 0x0006E9F0
		private string GetAllSimpleObscuredTypes()
		{
			string result = "Can't get the list, sorry :(";
			string types = string.Empty;
			if (string.IsNullOrEmpty(this.allSimpleObscuredTypes))
			{
				IEnumerable<Type> source = from t in Assembly.GetExecutingAssembly().GetTypes()
				where t.IsPublic && t.Namespace == "CodeStage.AntiCheat.ObscuredTypes" && t.Name != "ObscuredPrefs"
				select t;
				source.ToList<Type>().ForEach(delegate(Type t)
				{
					if (types.Length > 0)
					{
						types = types + "\n" + t.Name;
					}
					else
					{
						types += t.Name;
					}
				});
				if (!string.IsNullOrEmpty(types))
				{
					result = types;
					this.allSimpleObscuredTypes = types;
				}
			}
			else
			{
				result = this.allSimpleObscuredTypes;
			}
			return result;
		}

		// Token: 0x06000FB9 RID: 4025 RVA: 0x0000F228 File Offset: 0x0000D428
		private string GetAllObscuredPrefsDataTypes()
		{
			return "int\nfloat\nstring\n<color=\"#75C4EB\">uint\ndouble\nlong\nbool\nbyte[]\nVector2\nVector3\nQuaternion\nColor\nRect</color>";
		}

		// Token: 0x06000FBA RID: 4026 RVA: 0x00070898 File Offset: 0x0006EA98
		private void LoadRegularPrefs()
		{
			this.regularPrefs = "int: " + PlayerPrefs.GetInt("money", -1) + "\n";
			string text = this.regularPrefs;
			this.regularPrefs = string.Concat(new object[]
			{
				text,
				"float: ",
				PlayerPrefs.GetFloat("lifeBar", -1f),
				"\n"
			});
			this.regularPrefs = this.regularPrefs + "string: " + PlayerPrefs.GetString("name", "No saved PlayerPrefs!");
		}

		// Token: 0x06000FBB RID: 4027 RVA: 0x0000F22F File Offset: 0x0000D42F
		private void SaveRegularPrefs()
		{
			PlayerPrefs.SetInt("money", 456);
			PlayerPrefs.SetFloat("lifeBar", 456.789f);
			PlayerPrefs.SetString("name", "Hey, there!");
			PlayerPrefs.Save();
		}

		// Token: 0x06000FBC RID: 4028 RVA: 0x0000F263 File Offset: 0x0000D463
		private void DeleteRegularPrefs()
		{
			PlayerPrefs.DeleteKey("money");
			PlayerPrefs.DeleteKey("lifeBar");
			PlayerPrefs.DeleteKey("name");
			PlayerPrefs.Save();
		}

		// Token: 0x06000FBD RID: 4029 RVA: 0x00070934 File Offset: 0x0006EB34
		private void LoadObscuredPrefs()
		{
			byte[] byteArray = ObscuredPrefs.GetByteArray("demoByteArray", 0, 4);
			this.obscuredPrefs = "int: " + ObscuredPrefs.GetInt("money", -1) + "\n";
			string text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"float: ",
				ObscuredPrefs.GetFloat("lifeBar", -1f),
				"\n"
			});
			this.obscuredPrefs = this.obscuredPrefs + "string: " + ObscuredPrefs.GetString("name", "No saved ObscuredPrefs!") + "\n";
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"bool: ",
				ObscuredPrefs.GetBool("gameComplete", false),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"uint: ",
				ObscuredPrefs.GetUInt("demoUint", 0U),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"long: ",
				ObscuredPrefs.GetLong("demoLong", -1L),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"double: ",
				ObscuredPrefs.GetDouble("demoDouble", -1.0),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"Vector2: ",
				ObscuredPrefs.GetVector2("demoVector2", Vector2.zero),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"Vector3: ",
				ObscuredPrefs.GetVector3("demoVector3", Vector3.zero),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"Quaternion: ",
				ObscuredPrefs.GetQuaternion("demoQuaternion", Quaternion.identity),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"Rect: ",
				ObscuredPrefs.GetRect("demoRect", new Rect(0f, 0f, 0f, 0f)),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"Color: ",
				ObscuredPrefs.GetColor("demoColor", Color.black),
				"\n"
			});
			text = this.obscuredPrefs;
			this.obscuredPrefs = string.Concat(new object[]
			{
				text,
				"byte[]: {",
				byteArray[0],
				",",
				byteArray[1],
				",",
				byteArray[2],
				",",
				byteArray[3],
				"}"
			});
		}

		// Token: 0x06000FBE RID: 4030 RVA: 0x00070CC0 File Offset: 0x0006EEC0
		private void SaveObscuredPrefs()
		{
			ObscuredPrefs.SetInt("money", 123);
			ObscuredPrefs.SetFloat("lifeBar", 123.456f);
			ObscuredPrefs.SetString("name", "Goscurry is not a lie ;)");
			ObscuredPrefs.SetBool("gameComplete", true);
			ObscuredPrefs.SetUInt("demoUint", 1234567891U);
			ObscuredPrefs.SetLong("demoLong", 1234567891234567890L);
			ObscuredPrefs.SetDouble("demoDouble", 1.234567890123456);
			ObscuredPrefs.SetVector2("demoVector2", Vector2.one);
			ObscuredPrefs.SetVector3("demoVector3", Vector3.one);
			ObscuredPrefs.SetQuaternion("demoQuaternion", Quaternion.Euler(new Vector3(10f, 20f, 30f)));
			ObscuredPrefs.SetRect("demoRect", new Rect(1.5f, 2.6f, 3.7f, 4.8f));
			ObscuredPrefs.SetColor("demoColor", Color.red);
			ObscuredPrefs.SetByteArray("demoByteArray", new byte[]
			{
				44,
				104,
				43,
				32
			});
			ObscuredPrefs.Save();
		}

		// Token: 0x06000FBF RID: 4031 RVA: 0x00070DD0 File Offset: 0x0006EFD0
		private void DeleteObscuredPrefs()
		{
			ObscuredPrefs.DeleteKey("money");
			ObscuredPrefs.DeleteKey("lifeBar");
			ObscuredPrefs.DeleteKey("name");
			ObscuredPrefs.DeleteKey("gameComplete");
			ObscuredPrefs.DeleteKey("demoUint");
			ObscuredPrefs.DeleteKey("demoLong");
			ObscuredPrefs.DeleteKey("demoDouble");
			ObscuredPrefs.DeleteKey("demoVector2");
			ObscuredPrefs.DeleteKey("demoVector3");
			ObscuredPrefs.DeleteKey("demoQuaternion");
			ObscuredPrefs.DeleteKey("demoRect");
			ObscuredPrefs.DeleteKey("demoColor");
			ObscuredPrefs.DeleteKey("demoByteArray");
			ObscuredPrefs.Save();
		}

		// Token: 0x06000FC0 RID: 4032 RVA: 0x0000F288 File Offset: 0x0000D488
		private void PlaceUrlButton(string url)
		{
			this.PlaceUrlButton(url, 30);
		}

		// Token: 0x06000FC1 RID: 4033 RVA: 0x0000F293 File Offset: 0x0000D493
		private void PlaceUrlButton(string url, int width)
		{
			this.PlaceUrlButton(url, "?", width);
		}

		// Token: 0x06000FC2 RID: 4034 RVA: 0x00070E64 File Offset: 0x0006F064
		private void PlaceUrlButton(string url, string buttonName, int width)
		{
			GUILayoutOption[] array = new GUILayoutOption[1];
			if (width != -1)
			{
				array[0] = GUILayout.Width((float)width);
			}
			else
			{
				array = null;
			}
			if (GUILayout.Button(buttonName, array))
			{
				Application.OpenURL(url);
			}
		}

		// Token: 0x06000FC3 RID: 4035 RVA: 0x0000F2A2 File Offset: 0x0000D4A2
		private void OnApplicationQuit()
		{
			this.DeleteRegularPrefs();
			this.DeleteObscuredPrefs();
		}

		// Token: 0x04000A92 RID: 2706
		private const string RED_COLOR = "#FF4040";

		// Token: 0x04000A93 RID: 2707
		private const string GREEN_COLOR = "#02C85F";

		// Token: 0x04000A94 RID: 2708
		private const string PREFS_STRING = "name";

		// Token: 0x04000A95 RID: 2709
		private const string PREFS_INT = "money";

		// Token: 0x04000A96 RID: 2710
		private const string PREFS_FLOAT = "lifeBar";

		// Token: 0x04000A97 RID: 2711
		private const string PREFS_BOOL = "gameComplete";

		// Token: 0x04000A98 RID: 2712
		private const string PREFS_UINT = "demoUint";

		// Token: 0x04000A99 RID: 2713
		private const string PREFS_LONG = "demoLong";

		// Token: 0x04000A9A RID: 2714
		private const string PREFS_DOUBLE = "demoDouble";

		// Token: 0x04000A9B RID: 2715
		private const string PREFS_VECTOR2 = "demoVector2";

		// Token: 0x04000A9C RID: 2716
		private const string PREFS_VECTOR3 = "demoVector3";

		// Token: 0x04000A9D RID: 2717
		private const string PREFS_QUATERNION = "demoQuaternion";

		// Token: 0x04000A9E RID: 2718
		private const string PREFS_RECT = "demoRect";

		// Token: 0x04000A9F RID: 2719
		private const string PREFS_COLOR = "demoColor";

		// Token: 0x04000AA0 RID: 2720
		private const string PREFS_BYTE_ARRAY = "demoByteArray";

		// Token: 0x04000AA1 RID: 2721
		private const string API_URL_LOCK_TO_DEVICE = "http://j.mp/1gxg1tf";

		// Token: 0x04000AA2 RID: 2722
		private const string API_URL_PRESERVE_PREFS = "http://j.mp/1iBK5pz";

		// Token: 0x04000AA3 RID: 2723
		private const string API_URL_EMERGENCY_MODE = "http://j.mp/1FRAL5L";

		// Token: 0x04000AA4 RID: 2724
		private const string API_URL_READ_FOREIGN = "http://j.mp/1LCdpDa";

		// Token: 0x04000AA5 RID: 2725
		private const string API_URL_UNOBSCURED_MODE = "http://j.mp/1KVrpxi";

		// Token: 0x04000AA6 RID: 2726
		private const string API_URL_PLAYER_PREFS = "http://docs.unity3d.com/ScriptReference/PlayerPrefs.html";

		// Token: 0x04000AA7 RID: 2727
		[Header("Regular variables")]
		public string regularString = "I'm regular string";

		// Token: 0x04000AA8 RID: 2728
		public int regularInt = 1987;

		// Token: 0x04000AA9 RID: 2729
		public float regularFloat = 2013.05237f;

		// Token: 0x04000AAA RID: 2730
		public Vector3 regularVector3 = new Vector3(10.5f, 11.5f, 12.5f);

		// Token: 0x04000AAB RID: 2731
		[Header("Obscured (secure) variables")]
		public ObscuredString obscuredString = "I'm obscured string";

		// Token: 0x04000AAC RID: 2732
		public ObscuredInt obscuredInt = 1987;

		// Token: 0x04000AAD RID: 2733
		public ObscuredFloat obscuredFloat = 2013.05237f;

		// Token: 0x04000AAE RID: 2734
		public ObscuredVector3 obscuredVector3 = new Vector3(10.5f, 11.5f, 12.5f);

		// Token: 0x04000AAF RID: 2735
		public ObscuredBool obscuredBool = true;

		// Token: 0x04000AB0 RID: 2736
		public ObscuredLong obscuredLong = 945678987654123345L;

		// Token: 0x04000AB1 RID: 2737
		public ObscuredDouble obscuredDouble = 9.45678987654;

		// Token: 0x04000AB2 RID: 2738
		public ObscuredVector2 obscuredVector2 = new Vector2(8.5f, 9.5f);

		// Token: 0x04000AB3 RID: 2739
		[Header("Other")]
		public string prefsEncryptionKey = "change me!";

		// Token: 0x04000AB4 RID: 2740
		private readonly string[] tabs = new string[]
		{
			"Variables protection",
			"Saves protection",
			"Cheating detectors"
		};

		// Token: 0x04000AB5 RID: 2741
		private int currentTab;

		// Token: 0x04000AB6 RID: 2742
		private string allSimpleObscuredTypes;

		// Token: 0x04000AB7 RID: 2743
		private string regularPrefs;

		// Token: 0x04000AB8 RID: 2744
		private string obscuredPrefs;

		// Token: 0x04000AB9 RID: 2745
		private int savesLock;

		// Token: 0x04000ABA RID: 2746
		private bool savesAlterationDetected;

		// Token: 0x04000ABB RID: 2747
		private bool foreignSavesDetected;

		// Token: 0x04000ABC RID: 2748
		private bool injectionDetected;

		// Token: 0x04000ABD RID: 2749
		private bool speedHackDetected;

		// Token: 0x04000ABE RID: 2750
		private bool obscuredTypeCheatDetected;

		// Token: 0x04000ABF RID: 2751
		private bool wallHackCheatDetected;

		// Token: 0x04000AC0 RID: 2752
		private readonly StringBuilder logBuilder = new StringBuilder();
	}
}
