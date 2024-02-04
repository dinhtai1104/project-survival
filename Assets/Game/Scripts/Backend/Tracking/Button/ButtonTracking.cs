namespace tracking.button
{
    public class ButtonTracking : UIBaseButton
    {
        public string category;
        public string nameButton;
        public override void Action()
        {
            FirebaseAnalysticController.Tracker.NewEvent("button_click")
                .AddStringParam("category", category.ToString())
                .AddStringParam("name", this.nameButton.ToString())
                .Track();
        }
    }
}
