using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BSCM.Game{
	public enum GameMode{
		TeamDeathmatch = 0,
		Classic = 1,
		KnifeMode = 2,
		AWPMode = 3,
		GunGame = 4,
		//DeathRun = 5,
		RandomMode = 8,
		Deathmatch = 9,
		BunnyHop = 10,
		Bomb = 16,
		Bomb2 = 19,
	}

	public enum Team{
		None,
		Blue,
		Red
	}
}
