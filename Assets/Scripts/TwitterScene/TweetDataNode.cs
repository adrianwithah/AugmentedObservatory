[System.Serializable]
public class TweetDataNode {

	public string ID;
	public PosData Pos;
	public string Label;
	public double Size;
	public int R;
	public int G;
	public int B;
	public AttrsData Attrs;
	public int NumLinks;
	public string[] Adj;

	[System.Serializable]
	public class PosData {
		public double X;
		public double Y;
	}

	[System.Serializable]
	public class AttrsData {
		public string username;
		public string user_score;
		public string date;
		public string tweet;
		public string tweet_score;
		public string gravity_y_strength;
		public string gravity_x_strength;
		public string node_type;
		public string label;
		public string gravity_x;
		public string gravity_y;
		public string time;
		public string type;
	}

	public void Translate(float deltaX, float deltaY) {
		this.Pos.X += deltaX;
		this.Pos.Y += deltaY;
	}
}






