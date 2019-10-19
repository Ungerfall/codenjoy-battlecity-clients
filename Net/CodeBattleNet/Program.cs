using System;
using System.Collections.Generic;
using CodeBattleNetLibrary;
using static CodeBattleNet.Configuration;

namespace CodeBattleNet
{
    internal static class Program
    {
		static GameClientBattlecity G;
		static Stats S = new Stats();
        private static void Main()
        {
            G = new GameClientBattlecity("http://dojorena.io/codenjoy-contest/board/player/int06uj890zd3heph6z8?code=2017086748052372503&gameName=battlecity");
			Log("GAME STARTED. MAP: " + G.MapSize);
            G.Run(() =>
            {
                Move();
            });
            Console.Read();
        }

		static Elements[,] PrevTurnMap;
        private static void Move()
        {
			Log("New move");
			var sw = new System.Diagnostics.Stopwatch();
			sw.Start();
			if (PrevTurnMap == null)
				PrevTurnMap = G.Map;
            var done = false;

			done = Dodge();
			if (!done && S.ActCooldown <= 0)
				done = ActAvailableEnemy();
			if (!done)
				done = MoveTank();
			if (!done)
			{
				S.ActInactive++;
				if (!ActBarier())
					G.SendActions(G.Blank());
			}

			S.ActCooldown--;
			PrevTurnMap = G.Map;
			LogStats();
			Log(" " + sw.ElapsedMilliseconds);
			Log(Environment.NewLine);
		}

		static bool Dodge()
		{
			const int maxObserveLen = Configuration.DodgeLen;
			// right line
			for (int x = G.PlayerX + 1; x < Math.Min(G.Map.GetLength(0), G.PlayerX + maxObserveLen); x++)
			{
				if (G.IsAnyOfAt(x, G.PlayerY, new[] { Elements.BULLET }))
				{
					if (!G.IsBarrierAt(x, G.PlayerY - 1))
					{
						G.SendActions(G.Up());
						S.DodgeUp++;
						return true;
					}
					else
					{
						G.SendActions(G.Down());
						S.DodgeDown++;
						return true;
					}
				}
			}
			// left line
			for (int x = G.PlayerX - 1; x >= Math.Max(0, G.PlayerX - maxObserveLen); x--)
			{
				if (G.IsAnyOfAt(x, G.PlayerY, new[] { Elements.BULLET }))
				{
					if (!G.IsBarrierAt(x, G.PlayerY - 1))
					{
						G.SendActions(G.Up());
						S.DodgeUp++;
						return true;
					}
					else
					{
						G.SendActions(G.Down());
						S.DodgeDown++;
						return true;
					}
				}
			}
			// down line
			for (int y = G.PlayerY + 1; y <  Math.Min(G.Map.GetLength(1), G.PlayerY + maxObserveLen); y++)
			{
				if (G.IsAnyOfAt(G.PlayerX, y, new[] { Elements.BULLET }))
				{
					if (!G.IsBarrierAt(G.PlayerX + 1, y))
					{
						G.SendActions(G.Right());
						S.DodgeRight++;
						return true;
					}
					else
					{
						G.SendActions(G.Left());
						S.DodgeLeft++;
						return true;
					}
				}
			}
			// up line
			for (int y = G.PlayerY - 1; y >= Math.Max(0, G.PlayerY - maxObserveLen); y--)
			{
				if (G.IsAnyOfAt(G.PlayerX, y, new[] { Elements.BULLET }))
				{
					if (!G.IsBarrierAt(G.PlayerX + 1, y))
					{
						G.SendActions(G.Right());
						S.DodgeRight++;
						return true;
					}
					else
					{
						G.SendActions(G.Left());
						S.DodgeLeft++;
						return true;
					}
				}
			}

			return false;
		}

		static bool MoveTank()
		{
			var r = new Random();
			switch (r.Next(100))
			{
				case int n when (n < MoveUpWeight):
					if (!G.IsBarrierAt(G.PlayerX, G.PlayerY - 1))
					{
						G.SendActions(G.Up());
						S.MoveUp++;
						return true;
					}
					else
					{
						break;
					}
				case int n when (n < MoveUpWeight + MoveRightWeight):
					if (!G.IsBarrierAt(G.PlayerX + 1, G.PlayerY))
					{
						G.SendActions(G.Right());
						S.MoveRight++;
						return true;
					}
					else
					{
						break;
					}
				case int n when (n < MoveUpWeight + MoveRightWeight + MoveDownWeight):
					if (!G.IsBarrierAt(G.PlayerX, G.PlayerY + 1))
					{
						G.SendActions(G.Down());
						S.MoveDown++;
						return true;
					}
					else
					{
						break;
					}
				case int n when (n < 100):
					if (!G.IsBarrierAt(G.PlayerX - 1, G.PlayerY))
					{
						G.SendActions(G.Left());
						S.MoveLeft++;
						return true;
					}
					break;
				default:
					return false;
			}

			return false;
		}

		private static bool ActAvailableEnemy()
		{
			var done = false;
			const int maxObserveLen = Configuration.ActLen;
			for (int x = G.PlayerX + 1; x < Math.Min(G.Map.GetLength(0), G.PlayerX + maxObserveLen); x++)
			{
				if (G.IsAnyOfAt(x, G.PlayerY, EnemyTankElements))
				{
					G.SendActions(G.Right() + "," + G.Act());
					S.ActRight++;
					done = true;
				}

				if (G.IsBarrierAt(x, G.PlayerY) || G.IsOutOf(x, G.PlayerY))
					break;
			}
			for (int x = G.PlayerX - 1; x >= Math.Max(0, G.PlayerX - maxObserveLen); x--)
			{
				if (G.IsAnyOfAt(x, G.PlayerY, EnemyTankElements))
				{
					G.SendActions(G.Left() + "," + G.Act());
					S.ActLeft++;
					done = true;
				}

				if (G.IsBarrierAt(x, G.PlayerY))
					break;
			}
			for (int y = G.PlayerY + 1; y <  Math.Min(G.Map.GetLength(1), G.PlayerY + maxObserveLen); y++)
			{
				if (G.IsAnyOfAt(G.PlayerX, y, EnemyTankElements))
				{
					G.SendActions(G.Down() + "," + G.Act());
					S.ActDown++;
					done = true;
				}

				if (G.IsBarrierAt(G.PlayerX, y))
					break;
			}
			for (int y = G.PlayerY - 1; y >= Math.Max(0, G.PlayerY - maxObserveLen); y--)
			{
				if (G.IsAnyOfAt(G.PlayerX, y, EnemyTankElements))
				{
					G.SendActions(G.Up() + "," + G.Act());
					S.ActUp++;
					done = true;
				}

				if (G.IsBarrierAt(G.PlayerX, y))
					break;
			}

			if (done)
			{
				S.ActCooldown = 4;
			}

			return done;
		}

		static bool ActBarier()
		{
			if (G.IsAnyOfAt(G.PlayerX, G.PlayerY + 1, Destructible))
			{
				G.SendActions(G.Down() + "," + G.Act());
				return true;
			}

			if (G.IsAnyOfAt(G.PlayerX - 1, G.PlayerY, Destructible))
			{
				G.SendActions(G.Left() + "," + G.Act());
				return true;
			}

			if (G.IsAnyOfAt(G.PlayerX + 1, G.PlayerY, Destructible))
			{
				G.SendActions(G.Right() + "," + G.Act());
				return true;
			}

			if (G.IsAnyOfAt(G.PlayerX, G.PlayerY - 1, Destructible))
			{
				G.SendActions(G.Up() + "," + G.Act());
				return true;
			}

			return false;
		}

		private static Elements[] EnemyTankElements =>
			new Elements[]
			{
				Elements.OTHER_TANK_DOWN,
				Elements.OTHER_TANK_LEFT,
				Elements.OTHER_TANK_UP,
				Elements.OTHER_TANK_RIGHT,
				Elements.AI_TANK_DOWN,
				Elements.AI_TANK_LEFT,
				Elements.AI_TANK_RIGHT,
				Elements.AI_TANK_UP,
			};

		private static Elements[] Destructible =>
			new Elements[]
			{
				Elements.CONSTRUCTION,
				Elements.CONSTRUCTION_DESTROYED_DOWN,
				Elements.CONSTRUCTION_DESTROYED_UP,
				Elements.CONSTRUCTION_DESTROYED_LEFT,
				Elements.CONSTRUCTION_DESTROYED_RIGHT,
				Elements.CONSTRUCTION_DESTROYED_DOWN_TWICE,
				Elements.CONSTRUCTION_DESTROYED_UP_TWICE,
				Elements.CONSTRUCTION_DESTROYED_LEFT_TWICE,
				Elements.CONSTRUCTION_DESTROYED_RIGHT_TWICE,
				Elements.CONSTRUCTION_DESTROYED_LEFT_RIGHT,
				Elements.CONSTRUCTION_DESTROYED_UP_DOWN,
				Elements.CONSTRUCTION_DESTROYED_UP_LEFT,
				Elements.CONSTRUCTION_DESTROYED_RIGHT_UP,
				Elements.CONSTRUCTION_DESTROYED_DOWN_LEFT,
				Elements.CONSTRUCTION_DESTROYED_DOWN_RIGHT,
			};

		static void LogStats()
		{
			Console.Write(S);
		}

		static void Log(string msg)
		{
			Console.Write(msg);
		}

		public enum Dir
		{
			Up,
			Down,
			Left,
			Right
		}
    }
}
