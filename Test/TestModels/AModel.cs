namespace TestModels
{
	public class AModel
	{
		public string[] APublicProperty { get; set; }
		
		private bool APrivateProperty { get; set; }

		public string AMethod()
		{
			return "A Method";
		}
	}
}