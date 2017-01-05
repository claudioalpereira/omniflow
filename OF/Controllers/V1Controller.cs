using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Text;
using System.Web.Http;

namespace OF.Controllers
{
    public class V1Controller : ApiController
    {
        private static string _lastInstData;
        private static DateTime _lastInstDataDT;

        // GET: api/V1
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/V1/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/V1
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/V1/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/V1/5
        public void Delete(int id)
        {
        }

        [Route("api/v1/getinstdata")]
        public string GetInstData()
        {
            if(_lastInstDataDT!=null && DateTime.Now < _lastInstDataDT.AddMinutes(1))
            {
                return _lastInstData;
            }

            QueryServer("+updateinst");
            System.Threading.Thread.Sleep(15000);

            var instData = QueryServer("+getinstdata");
            var json = System.Web.Helpers.Json.Decode(instData);

            _lastInstDataDT = DateTime.Now;
            _lastInstData = instData;

            return instData;
        }

        [Route("api/v1/getconfdata")]
        public string GetConfData()
        {
            return QueryServer("+getconfdata");
        }

        [Route("api/v1/getstate")]
        public string GetState()
        {
            return QueryServer("+getstate");
        }

        [Route("api/v1/getmeasures")]
        public string GetState(DateTime from, DateTime to)
        {
            return QueryServer(string.Format("+getmeasures[{0:yyyy-MM-dd},{1:yyyy-MM-dd}]", from, to));
        }




        private string QueryServer(string request)
        {

            byte[] bytes = new byte[128*1024];
            string ret = null;

            try
            {
                IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("62.28.231.130"), 19000);

                Socket sok = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    sok.Connect(ipep);

                    byte[] msg = Encoding.ASCII.GetBytes("+login[357976063980593 , Siemens , Omni2016]");

                    int bytesSent = sok.Send(msg);

                    int bytesRec = sok.Receive(bytes);

                    bytesSent = sok.Send(Encoding.ASCII.GetBytes(request));

                    bytesRec = sok.Receive(bytes);


                    sok.Shutdown(SocketShutdown.Both);
                    sok.Close();

                    ret = Encoding.ASCII.GetString(bytes, 0, bytesRec);
                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());

                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());

                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return ret;
        }
    }
}
