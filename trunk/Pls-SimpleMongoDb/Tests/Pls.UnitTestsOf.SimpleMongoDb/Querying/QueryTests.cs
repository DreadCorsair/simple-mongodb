using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pls.SimpleMongoDb.Querying;

namespace Pls.UnitTestsOf.SimpleMongoDb.Querying
{
    [TestClass]
    public class QueryTests
    {
        [TestMethod]
        public void In_SeveralStringOperands_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Name"].In("Daniel", "Sue"));
            
            Assert.AreEqual("{ Name : { $in : ['Daniel','Sue'] } }", query.ToString());
        }

        [TestMethod]
        public void In_ChainedOnSameProperty_BuildsCorrectFormat()
        {
            Assert.Inconclusive("TBD");
            //var query = Query.Create()["Name"].In("Daniel").In("Sue");

            //Assert.AreEqual("{ Name : { $in : ['Daniel','Sue'] } }", query.ToString());
        }

        [TestMethod]
        public void In_SeveralIntOperands_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Name"].In(21, 22));

            Assert.AreEqual("{ Name : { $in : [21,22] } }", query.ToString());
        }

        [TestMethod]
        public void InAndIn_WithStringsAndInts_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Name"].In("Daniel", "Sue").And("Age").In(21, 22));

            Assert.AreEqual("{ Name : { $in : ['Daniel','Sue'] }, Age : { $in : [21,22] } }", query.ToString());
        }

        [TestMethod]
        public void Where_WithStringAndInt_BuildsCorrectFormat()
        {
            var query = Query.New(q => q.Where(@"this.Name == 'Daniel' || this.Age == 21"));

            Assert.AreEqual(@"{ $where : "" this.Name == 'Daniel' || this.Age == 21 "" }", query.ToString());
        }

        [TestMethod]
        public void InAndWhere_InFirstAndWhereSecond_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Name"].In("Daniel", "Sue").And().Where(@"this.Age == 21 || this.Age == 22"));

            Assert.AreEqual(@"{ Name : { $in : ['Daniel','Sue'] }, $where : "" this.Age == 21 || this.Age == 22 "" }", query.ToString());
        }

        [TestMethod]
        public void InAndWhere_WhereFirstAndInSecond_BuildsCorrectFormat()
        {
            var query = Query.New(q => q.Where(@"this.Age == 21 || this.Age == 22").And("Name").In("Daniel", "Sue"));

            Assert.AreEqual(@"{ $where : "" this.Age == 21 || this.Age == 22 "", Name : { $in : ['Daniel','Sue'] } }", query.ToString());
        }

        [TestMethod]
        public void Lt_WhitInt_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Age"].Lt(50));

            Assert.AreEqual(@"{ Age : { $lt : 50 } }", query.ToString());
        }

        [TestMethod]
        public void LtE_WhitInt_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Age"].LtE(50));

            Assert.AreEqual(@"{ Age : { $lte : 50 } }", query.ToString());
        }

        [TestMethod]
        public void Gt_WhitInt_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Age"].Gt(50));

            Assert.AreEqual(@"{ Age : { $gt : 50 } }", query.ToString());
        }

        [TestMethod]
        public void GtE_WhitInt_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Age"].GtE(50));

            Assert.AreEqual(@"{ Age : { $gte : 50 } }", query.ToString());
        }

        [TestMethod]
        public void InAndLt_ChainedOnSameProperty_BuildsCorrectFormat()
        {
            var query = Query.New(q => q["Age"].In(21, 22, 23).Lt(23));
            
            Assert.AreEqual(@"{ Age : { $in : [21,22,23], $lt : 23 } }", query.ToString());
        }
    }
}