namespace JieNor.Megi.DataModel.GL
{
	public static class CheckGroupValueExtension
	{
		public static GLCheckGroupValueModel Clone(this GLCheckGroupValueModel model)
		{
			return new GLCheckGroupValueModel
			{
				MContactID = model.MContactID,
				MEmployeeID = model.MEmployeeID,
				MMerItemID = model.MMerItemID,
				MExpItemID = model.MExpItemID,
				MPaItemID = model.MPaItemID,
				MTrackItem1 = model.MTrackItem1,
				MTrackItem2 = model.MTrackItem2,
				MTrackItem3 = model.MTrackItem3,
				MTrackItem4 = model.MTrackItem4,
				MTrackItem5 = model.MTrackItem5,
				MOrgID = model.MOrgID
			};
		}
	}
}
