namespace CodeBattleNet
{
	public class Stats
	{
		public int ActUp { get; set; }
		public int ActDown { get; set; }
		public int ActLeft { get; set; }
		public int ActRight { get; set; }

		public int MoveUp { get; set; }
		public int MoveDown { get; set; }
		public int MoveLeft { get; set; }
		public int MoveRight { get; set; }

		public int DodgeUp { get; set; }
		public int DodgeDown { get; set; }
		public int DodgeLeft { get; set; }
		public int DodgeRight { get; set; }

		public int ActInactive { get; set; }
		public int ActCooldown { get; set; }

		public override string ToString()
		{
			return $"{nameof(ActUp)}: {ActUp};{nameof(ActDown)}: {ActDown};"
				+ $"{nameof(ActLeft)}: {ActLeft};{nameof(ActRight)}: {ActRight};"
				+ System.Environment.NewLine
				+ $"{nameof(MoveUp)}: {MoveUp};{nameof(MoveDown)}: {MoveDown};"
				+ $"{nameof(MoveLeft)}: {MoveLeft};{nameof(MoveRight)}: {MoveRight};"
				+ System.Environment.NewLine
				+ $"{nameof(DodgeUp)}: {DodgeUp};{nameof(DodgeDown)}: {DodgeDown};"
				+ $"{nameof(DodgeLeft)}: {DodgeLeft};{nameof(DodgeRight)}: {DodgeRight};"
				+ System.Environment.NewLine
				+ $"{nameof(ActInactive)}: {ActInactive};{nameof(ActCooldown)}: {ActCooldown}"
				+ System.Environment.NewLine;
		}
	}
}
