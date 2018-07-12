using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace ApiXUnitTestProject
{
    public class UnitTest1
    {
        [Fact]                         
        public void DemoTestAssertExtensions()         
        {
            //SETUP
            const int someValue = 1;    

            //ATTEMPT
            var result = someValue * 2;  

            //VERIFY
            result.ShouldEqual(2);       
        }
    }
}
