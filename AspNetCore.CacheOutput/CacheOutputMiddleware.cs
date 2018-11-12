﻿using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.CacheOutput
{
    public class CacheOutputMiddleware
    {
        protected RequestDelegate Next;

        public CacheOutputMiddleware(RequestDelegate next)
        {
            this.Next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            Stream originalStream = context.Response.Body;

            try
            {
                using (Stream stream = new MemoryStream())
                {
                    context.Response.Body = stream;

                    await Next.Invoke(context);

                    stream.Seek(0, SeekOrigin.Begin);

                    await stream.CopyToAsync(originalStream);
                }
            }
            finally
            {
                context.Response.Body = originalStream;
            }
        }
    }
}
