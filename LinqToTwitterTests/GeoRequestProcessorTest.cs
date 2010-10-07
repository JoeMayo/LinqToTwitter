using LinqToTwitter;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Collections;
using System;
using System.Linq;
using System.Threading;
using System.Globalization;
using System.Configuration;
using LinqToTwitterTests.Common;

namespace LinqToTwitterTests
{
    /// <summary>
    ///This is a test class for GeoRequestProcessorTest and is intended
    ///to contain all GeoRequestProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GeoRequestProcessorTest
    {
        #region Test Data

        string m_reverseResponse = @"<root type=""object"">
  <result type=""object"">
    <places type=""array"">
      <item type=""object"">
        <contained_within type=""array"">
          <item type=""object"">
            <url type=""string"">http://api.twitter.com/1/geo/id/5a110d312052166f.json</url>
            <country type=""string""></country>
            <bounding_box type=""object"">
              <type type=""string"">Polygon</type>
              <coordinates type=""array"">
                <item type=""array"">
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                </item>
              </coordinates>
            </bounding_box>
            <full_name type=""string"">San Francisco</full_name>
            <country_code type=""string"">US</country_code>
            <name type=""string"">San Francisco</name>
            <id type=""string"">5a110d312052166f</id>
            <place_type type=""string"">city</place_type>
          </item>
        </contained_within>
        <url type=""string"">http://api.twitter.com/1/geo/id/2b6ff8c22edd9576.json</url>
        <bounding_box type=""object"">
          <type type=""string"">Polygon</type>
          <coordinates type=""array"">
            <item type=""array"">
              <item type=""array"">
                <item type=""number"">-122.42284884</item>
                <item type=""number"">37.76893497</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.3964</item>
                <item type=""number"">37.76893497</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.3964</item>
                <item type=""number"">37.78752897</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.42284884</item>
                <item type=""number"">37.78752897</item>
              </item>
            </item>
          </coordinates>
        </bounding_box>
        <country type=""string""></country>
        <full_name type=""string"">SoMa, San Francisco</full_name>
        <name type=""string"">SoMa</name>
        <country_code type=""string"">US</country_code>
        <place_type type=""string"">neighborhood</place_type>
        <id type=""string"">2b6ff8c22edd9576</id>
      </item>
      <item type=""object"">
        <contained_within type=""array"">
          <item type=""object"">
            <url type=""string"">http://api.twitter.com/1/geo/id/5a110d312052166f.json</url>
            <country type=""string""></country>
            <bounding_box type=""object"">
              <type type=""string"">Polygon</type>
              <coordinates type=""array"">
                <item type=""array"">
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                </item>
              </coordinates>
            </bounding_box>
            <full_name type=""string"">San Francisco</full_name>
            <country_code type=""string"">US</country_code>
            <name type=""string"">San Francisco</name>
            <id type=""string"">5a110d312052166f</id>
            <place_type type=""string"">city</place_type>
          </item>
        </contained_within>
        <url type=""string"">http://api.twitter.com/1/geo/id/5c92ab5379de3839.json</url>
        <bounding_box type=""object"">
          <type type=""string"">Polygon</type>
          <coordinates type=""array"">
            <item type=""array"">
              <item type=""array"">
                <item type=""number"">-122.40348192</item>
                <item type=""number"">37.77752898</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.387436</item>
                <item type=""number"">37.77752898</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.387436</item>
                <item type=""number"">37.79448597</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.40348192</item>
                <item type=""number"">37.79448597</item>
              </item>
            </item>
          </coordinates>
        </bounding_box>
        <country type=""string""></country>
        <full_name type=""string"">South Beach</full_name>
        <name type=""string"">South Beach</name>
        <country_code type=""string"">US</country_code>
        <place_type type=""string"">neighborhood</place_type>
        <id type=""string"">5c92ab5379de3839</id>
      </item>
      <item type=""object"">
        <contained_within type=""array"">
          <item type=""object"">
            <url type=""string"">http://api.twitter.com/1/geo/id/5a110d312052166f.json</url>
            <country type=""string""></country>
            <bounding_box type=""object"">
              <type type=""string"">Polygon</type>
              <coordinates type=""array"">
                <item type=""array"">
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                </item>
              </coordinates>
            </bounding_box>
            <full_name type=""string"">San Francisco</full_name>
            <country_code type=""string"">US</country_code>
            <name type=""string"">San Francisco</name>
            <id type=""string"">5a110d312052166f</id>
            <place_type type=""string"">city</place_type>
          </item>
        </contained_within>
        <url type=""string"">http://api.twitter.com/1/geo/id/41bcb736f84a799e.json</url>
        <bounding_box type=""object"">
          <type type=""string"">Polygon</type>
          <coordinates type=""array"">
            <item type=""array"">
              <item type=""array"">
                <item type=""number"">-122.40618084</item>
                <item type=""number"">37.76405301</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.38151184</item>
                <item type=""number"">37.76405301</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.38151184</item>
                <item type=""number"">37.78199199</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.40618084</item>
                <item type=""number"">37.78199199</item>
              </item>
            </item>
          </coordinates>
        </bounding_box>
        <country type=""string""></country>
        <full_name type=""string"">Mission Bay</full_name>
        <name type=""string"">Mission Bay</name>
        <country_code type=""string"">US</country_code>
        <place_type type=""string"">neighborhood</place_type>
        <id type=""string"">41bcb736f84a799e</id>
      </item>
      <item type=""object"">
        <contained_within type=""array"">
          <item type=""object"">
            <url type=""string"">http://api.twitter.com/1/geo/id/5a110d312052166f.json</url>
            <country type=""string""></country>
            <bounding_box type=""object"">
              <type type=""string"">Polygon</type>
              <coordinates type=""array"">
                <item type=""array"">
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.70813196</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.35845384</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-122.51368188</item>
                    <item type=""number"">37.83245301</item>
                  </item>
                </item>
              </coordinates>
            </bounding_box>
            <full_name type=""string"">San Francisco</full_name>
            <country_code type=""string"">US</country_code>
            <name type=""string"">San Francisco</name>
            <id type=""string"">5a110d312052166f</id>
            <place_type type=""string"">city</place_type>
          </item>
        </contained_within>
        <url type=""string"">http://api.twitter.com/1/geo/id/d7ffb21e3a0ee55c.json</url>
        <bounding_box type=""object"">
          <type type=""string"">Polygon</type>
          <coordinates type=""array"">
            <item type=""array"">
              <item type=""array"">
                <item type=""number"">-122.41896192</item>
                <item type=""number"">37.78151103</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.39558712</item>
                <item type=""number"">37.78151103</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.39558712</item>
                <item type=""number"">37.79382996</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.41896192</item>
                <item type=""number"">37.79382996</item>
              </item>
            </item>
          </coordinates>
        </bounding_box>
        <country type=""string""></country>
        <full_name type=""string"">Downtown</full_name>
        <name type=""string"">Downtown</name>
        <country_code type=""string"">US</country_code>
        <place_type type=""string"">neighborhood</place_type>
        <id type=""string"">d7ffb21e3a0ee55c</id>
      </item>
      <item type=""object"">
        <contained_within type=""array"">
          <item type=""object"">
            <url type=""string"">http://api.twitter.com/1/geo/id/fbd6d2f5a4e4a15e.json</url>
            <country type=""string""></country>
            <bounding_box type=""object"">
              <type type=""string"">Polygon</type>
              <coordinates type=""array"">
                <item type=""array"">
                  <item type=""array"">
                    <item type=""number"">-124.482003</item>
                    <item type=""number"">32.528832</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-114.131211</item>
                    <item type=""number"">32.528832</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-114.131211</item>
                    <item type=""number"">42.009517</item>
                  </item>
                  <item type=""array"">
                    <item type=""number"">-124.482003</item>
                    <item type=""number"">42.009517</item>
                  </item>
                </item>
              </coordinates>
            </bounding_box>
            <full_name type=""string"">California</full_name>
            <country_code type=""string"">US</country_code>
            <name type=""string"">California</name>
            <id type=""string"">fbd6d2f5a4e4a15e</id>
            <place_type type=""string"">admin</place_type>
          </item>
        </contained_within>
        <url type=""string"">http://api.twitter.com/1/geo/id/5a110d312052166f.json</url>
        <bounding_box type=""object"">
          <type type=""string"">Polygon</type>
          <coordinates type=""array"">
            <item type=""array"">
              <item type=""array"">
                <item type=""number"">-122.51368188</item>
                <item type=""number"">37.70813196</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.35845384</item>
                <item type=""number"">37.70813196</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.35845384</item>
                <item type=""number"">37.83245301</item>
              </item>
              <item type=""array"">
                <item type=""number"">-122.51368188</item>
                <item type=""number"">37.83245301</item>
              </item>
            </item>
          </coordinates>
        </bounding_box>
        <country type=""string""></country>
        <full_name type=""string"">San Francisco</full_name>
        <name type=""string"">San Francisco</name>
        <country_code type=""string"">US</country_code>
        <place_type type=""string"">city</place_type>
        <id type=""string"">5a110d312052166f</id>
      </item>
    </places>
  </result>
  <query type=""object"">
    <type type=""string"">reverse_geocode</type>
    <url type=""string"">http://api.twitter.com/1/geo/reverse_geocode.json?lat=37.78215&amp;long=-122.40060&amp;accuracy=0&amp;granularity=neighborhood</url>
    <params type=""object"">
      <accuracy type=""number"">0</accuracy>
      <granularity type=""string"">neighborhood</granularity>
      <coordinates type=""object"">
        <type type=""string"">Point</type>
        <coordinates type=""array"">
          <item type=""number"">-122.40060</item>
          <item type=""number"">37.78215</item>
        </coordinates>
      </coordinates>
    </params>
  </query>
</root>";

        string m_idResponse = @"<root type=""object"">
  <place_type type=""string"">city</place_type>
  <url type=""string"">http://api.twitter.com/1/geo/id/5a110d312052166f.json</url>
  <country type=""string""></country>
  <bounding_box type=""object"">
    <type type=""string"">Polygon</type>
    <coordinates type=""array"">
      <item type=""array"">
        <item type=""array"">
          <item type=""number"">-122.51368188</item>
          <item type=""number"">37.70813196</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.35845384</item>
          <item type=""number"">37.70813196</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.35845384</item>
          <item type=""number"">37.83245301</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.51368188</item>
          <item type=""number"">37.83245301</item>
        </item>
      </item>
    </coordinates>
  </bounding_box>
  <polylines type=""array"">
    <item type=""string"">yzceFfenjVsX~oCs]bt@oNfJ_aBrq@omGfc@uW{B}oDsyEcfCinSfI}h@zdBmq@x|OdxBbrBt`@hF|`ARbdM</item>
  </polylines>
  <full_name type=""string"">San Francisco</full_name>
  <geometry type=""object"">
    <type type=""string"">Polygon</type>
    <coordinates type=""array"">
      <item type=""array"">
        <item type=""array"">
          <item type=""number"">-122.46628284</item>
          <item type=""number"">37.70813196</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.48948304</item>
          <item type=""number"">37.712232</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.497983</item>
          <item type=""number"">37.71713196</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.49978012</item>
          <item type=""number"">37.71960696</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.50787796</item>
          <item type=""number"">37.73529</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.51368188</item>
          <item type=""number"">37.77856803</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.51305692</item>
          <item type=""number"">37.78252398</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.47808292</item>
          <item type=""number"">37.81082799</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.37322716</item>
          <item type=""number"">37.83245301</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.36652288</item>
          <item type=""number"">37.83081303</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.35845384</item>
          <item type=""number"">37.81450503</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.377842</item>
          <item type=""number"">37.72782</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.38322616</item>
          <item type=""number"">37.70940303</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.39378208</item>
          <item type=""number"">37.70823096</item>
        </item>
        <item type=""array"">
          <item type=""number"">-122.46628284</item>
          <item type=""number"">37.70813196</item>
        </item>
      </item>
    </coordinates>
  </geometry>
  <name type=""string"">San Francisco</name>
  <id type=""string"">5a110d312052166f</id>
  <contained_within type=""array"">
    <item type=""object"">
      <place_type type=""string"">admin</place_type>
      <url type=""string"">http://api.twitter.com/1/geo/id/fbd6d2f5a4e4a15e.json</url>
      <country type=""string""></country>
      <bounding_box type=""object"">
        <type type=""string"">Polygon</type>
        <coordinates type=""array"">
          <item type=""array"">
            <item type=""array"">
              <item type=""number"">-124.482003</item>
              <item type=""number"">32.528832</item>
            </item>
            <item type=""array"">
              <item type=""number"">-114.131211</item>
              <item type=""number"">32.528832</item>
            </item>
            <item type=""array"">
              <item type=""number"">-114.131211</item>
              <item type=""number"">42.009517</item>
            </item>
            <item type=""array"">
              <item type=""number"">-124.482003</item>
              <item type=""number"">42.009517</item>
            </item>
          </item>
        </coordinates>
      </bounding_box>
      <full_name type=""string"">California</full_name>
      <name type=""string"">California</name>
      <id type=""string"">fbd6d2f5a4e4a15e</id>
      <country_code type=""string"">US</country_code>
    </item>
  </contained_within>
  <country_code type=""string"">US</country_code>
</root>";

        #endregion

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TestCulture.SetCulture();
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessReverseResultsTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>();

            List<Geo> actual = (List<Geo>)target.ProcessResults(m_reverseResponse);

            Assert.AreEqual(5, actual[0].Places.Count);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessReverseResultsCultureInsensitiveTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>();

            List<Geo> actual = (List<Geo>)target.ProcessResults(m_reverseResponse);

            Assert.AreEqual(5, actual[0].Places.Count);
        }

        /// <summary>
        ///A test for ProcessResults
        ///</summary>
        [TestMethod()]
        public void ProcessIDResultsTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>();

            List<Geo> actual = (List<Geo>)target.ProcessResults(m_idResponse);

            Assert.AreEqual("San Francisco", actual[0].Places[0].Name);
        }

        /// <summary>
        ///A test for GetParameters
        ///</summary>
        [TestMethod()]
        public void GetParametersTest()
        {
            var geoReqProc = new GeoRequestProcessor<Geo>();
            Expression<Func<Geo, bool>> expression =
                geo =>
                    geo.Type == GeoType.Reverse &&
                    geo.Accuracy == "10" &&
                    geo.Granularity == "city" &&
                    geo.Latitude == 37.78215 &&
                    geo.Longitude == -122.40060 &&
                    geo.MaxResults == 10 &&
                    geo.ID == "456" &&
                    geo.IP == "168.143.171.180";
            LambdaExpression lambdaExpression = expression as LambdaExpression;

            var queryParams = geoReqProc.GetParameters(lambdaExpression);

            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Type", ((int)GeoType.Reverse).ToString())));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Accuracy", "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Granularity", "city")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Latitude", "37.78215")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("Longitude", "-122.4006")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("MaxResults", "10")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("ID", "456")));
            Assert.IsTrue(
                queryParams.Contains(
                    new KeyValuePair<string, string>("IP", "168.143.171.180")));
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildReverseURLTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Reverse).ToString()},
                 {"Latitude", "37.78215"},
                 {"Longitude", "-122.40060"},
                 {"Accuracy", "city" },
                 {"Granularity", "10" },
                 {"MaxResults", "10" }
             };
            string expected = "https://api.twitter.com/1/geo/reverse_geocode.json?lat=37.78215&long=-122.40060&accuracy=city&granularity=10&max_results=10";
            string actual;
            actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildReverseURLWithoutLatLongTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Reverse).ToString()},
                 {"Accuracy", "city" },
                 {"Granularity", "10" },
                 {"MaxResults", "10" }
             };
            string actual = target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildIDURLTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.ID).ToString()},
                 {"ID", "7695dd2ec2f86f2b"},
             };
            string expected = "https://api.twitter.com/1/geo/id/7695dd2ec2f86f2b.json";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildIDURLWithoutIDTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.ID).ToString()},
             };
            string actual = target.BuildURL(parameters);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        public void BuildNearbyURLTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Nearby).ToString()},
                 {"IP", "168.143.171.180"},
             };
            string expected = "https://api.twitter.com/1/geo/nearby_places.json?ip=168.143.171.180";
            string actual = target.BuildURL(parameters);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for BuildURL
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void BuildNearbyURLWithoutArgsTest()
        {
            GeoRequestProcessor<Geo> target = new GeoRequestProcessor<Geo>() { BaseUrl = "https://api.twitter.com/1/" };
            var parameters = new Dictionary<string, string>
             {
                 {"Type", ((int) GeoType.Nearby).ToString()},
             };
            string actual = target.BuildURL(parameters);
        }
    }
}
