using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Linq;
using LinqToTwitter;
using LinqToTwitterTests.Common;

namespace LinqToTwitterTests
{
    [TestClass]
    public class GeometryTests
    {
        private string boundingBoxXml = @"
<bounding_box>
  <georss:polygon xmlns:georss=""http://www.georss.org/georss"">-16.0524045 -48.285982 -16.0524045 -47.307264 -15.500216 -47.307264 -15.500216 -48.285982</georss:polygon>
</bounding_box>";

        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }

        [TestMethod]
        public void CreateGeometry_Parses_XElement()
        {
            XElement geoElem = XElement.Parse(this.boundingBoxXml);

            Geometry geo = new Geometry().CreateGeometry(geoElem);

            Assert.AreEqual(4, geo.Coordinates.Count);
            Assert.AreEqual(-16.0524045, geo.Coordinates[0].Latitude);
            Assert.AreEqual(-48.285982, geo.Coordinates[0].Longitude);
        }
    }
}
