using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using System.Collections.Generic;

namespace ChatClient
{
    public class HTTPClientService
    {
        HttpClient HTTPClient;

        public HTTPClientService()
        {
            HTTPClient = new HttpClient();
        }

        async public Task<int> PostRequest(string requestUri, HttpContent content)   //возможно заменить на put
        {
            HttpResponseMessage response = await HTTPClient.PostAsync(requestUri, content);
            string fileIDAsString = await response.Content.ReadAsStringAsync();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return int.Parse(fileIDAsString);
            }
            else
            {
                throw new FileLoadException("Не удалось загрузить ресурс '" + requestUri + "' так как он уже существует");
            }
        }

        async public Task<byte[]> GetRequest(string requestUri)
        {
            HttpResponseMessage response = await HTTPClient.GetAsync(requestUri);
            byte[] result = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                result = await response.Content.ReadAsByteArrayAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("Не удалось скачать ресурс '" + requestUri + "' так как он не существует");
            }
            return result;
        }

        async public Task<ResourceInformation> HeadRequest(string requestUri)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Head, requestUri);
            HttpResponseMessage response = await HTTPClient.SendAsync(request);
            ResourceInformation result = null;
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string[] NameHeaderValue = (string[])response.Headers.GetValues("Name");
                string[] SizeHeaderValue = (string[])response.Headers.GetValues("Size");
                result = new ResourceInformation(NameHeaderValue[0], SizeHeaderValue[0]);
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("Не удалось получить информацию о ресурсе '" + requestUri + "' так как он не существует");
            }
            return result;
        }

        async public Task DeleteRequest(string requestUri)
        {
            HttpResponseMessage response = await HTTPClient.DeleteAsync(requestUri);
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException("Не удалось удалить ресурс '" + requestUri + "' так как он не существует");
            }
        }
    }
}
