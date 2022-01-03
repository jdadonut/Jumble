namespace Jumble.Tests
{
    public class TestClass
    {
        private TestSubclass _subclass = new();
        private string testString => _subclass.testString;
        
        public void TestMethod()
        {
            
        }
        public string GetTestString()
        {
            return testString;
        }

        
    }
    public class TestSubclass
    {
        public string testString = "unchanged";
    }
}