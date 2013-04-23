using System;
using BusinessLogic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var myTravelManager = new TravelManager(new Func<ITravelSearchEngine>[0]);

            myTravelManager.AddTravel("Budapest", "London", DateTime.Today.AddDays(3));
            myTravelManager.AddTravel("London", "Budapest", DateTime.Today.AddDays(4));
            myTravelManager.AddTravel("Budapest", "London", DateTime.Today.AddDays(10));
            myTravelManager.AddTravel("London", "Budapest", DateTime.Today.AddDays(11));
            myTravelManager.AddTravel("Budapest", "London", DateTime.Today.AddDays(20));
            myTravelManager.AddTravel("London", "Budapest", DateTime.Today.AddDays(21));

            var searchplans = myTravelManager.GetSearchPlans();
        }
    }
}
