//  ---------------------------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
// 
//  The MIT License (MIT)
// 
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
// 
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
// 
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//  ---------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProjetoModels
{
    internal class AzureOrderDataSource : IOrderDataSource
    {
        public async Task<IEnumerable<Negotiation>> GetAsync() =>
            await ApiHelper.GetAsync<IEnumerable<Negotiation>>("order");

        public async Task<Negotiation> GetAsync(Guid id) =>
            await ApiHelper.GetAsync<Negotiation>($"order/{id}");

        public async Task<IEnumerable<Negotiation>> GetAsync(Customer customer) =>
            await ApiHelper.GetAsync<IEnumerable<Negotiation>>($"order/customer/{customer.Id}");

        public async Task<IEnumerable<Negotiation>> GetAsync(string search) =>
            await ApiHelper.GetAsync<IEnumerable<Negotiation>>($"order/search?value={search}");

        public async Task<Negotiation> PostAsync(Negotiation order) =>
            await ApiHelper.PostAsync<Negotiation, Negotiation>("order", order);

        public async Task<HttpResponseMessage> DeleteAsync(Guid orderId) =>
            await ApiHelper.DeleteAsync<HttpResponseMessage>("order", orderId);
    }
}
