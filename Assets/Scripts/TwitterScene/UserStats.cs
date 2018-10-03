using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TweetStats {
	public UserStats User_Stats;
	public AggregateStats Aggregate_Stats;
}

[System.Serializable]
public class UserStats {
	public double Min_Y;
	public double Max_Y;
	public double Avg_Y;
	public double Num_Retweeted;
	public double Num_Leave;
	public double Num_Remain;
	public double Num_Unknown;
}

[System.Serializable]
public class AggregateStats {
	public int Highest;
	public int Lowest;
	public double Average;
}