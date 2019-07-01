namespace Netlenium.Driver.Chrome
{
    public class Actions : IActions
    {
        private readonly Driver driver;

        private readonly Navigation navigation;

        public Actions(Driver driver)
        {
            this.driver = driver;
            this.navigation = new Navigation(driver);

        }

        public INavigation Navigation => this.navigation;
    }
}
