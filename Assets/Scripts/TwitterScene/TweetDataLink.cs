using System.Text;

[System.Serializable]
public class TweetDataLink {
	public string Source;
	public string Target;
	public StartPosData StartPos;
	public EndPosData EndPos;
	public int Weight;
	public int R;
	public int G;
	public int B;
	public AttrsData Attrs;

	[System.Serializable]
	public class StartPosData {
		public double X;
		public double Y;
	}

	[System.Serializable]
	public class EndPosData {
		public double X;
		public double Y;
	}

	[System.Serializable]
	public class AttrsData {
		public string Edge_Id;
		public string b;
		public string g;
		public string r;
		public string type;
		public double weight;
	}

	public override string ToString() {
		StringBuilder sb = new StringBuilder();
		sb.AppendFormat("Source: {0}\n", Source);
		sb.AppendFormat("Target: {0}\n", Target);
		sb.AppendFormat("StartPos.x: {0}, StartPos.y: {1}\n", StartPos.X, StartPos.Y);
		sb.AppendFormat("EndPos.x: {0}, EndPos.y: {1}\n", EndPos.X, EndPos.Y);
		sb.AppendFormat("Weight: {0}\n", Weight);
		return sb.ToString();
	}

	public void Translate(float deltaX, float deltaY) {
		this.StartPos.X += deltaX;
		this.StartPos.Y += deltaY;
		this.EndPos.X += deltaX;
		this.EndPos.Y += deltaY;
	}
}
