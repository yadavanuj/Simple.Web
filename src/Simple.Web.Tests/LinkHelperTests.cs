﻿namespace Simple.Web.Tests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Links;
    using Xunit;

    public class LinkHelperTests
    {
        [Fact]
        public void BuildsLinkUsingCustomUriTemplate()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetLinksForModel(customer).Single(l => l.GetHandlerType() == typeof(CustomerOrdersHandler));
            Assert.NotNull(link);
            Assert.Equal("/customers/42/orders", link.Href);
            Assert.Equal("customer.orders", link.Rel);
            Assert.Equal("application/vnd.list.order", link.Type);
        }
        
        [Fact]
        public void BuildsLinkUsingDefaultUriTemplate()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetLinksForModel(customer).Single(l => l.GetHandlerType() == typeof(CustomerLocationHandler));
            Assert.NotNull(link);
            Assert.Equal("/customers/42/location", link.Href);
            Assert.Equal("customer.location", link.Rel);
            Assert.Equal("application/vnd.location", link.Type);
        }
        
        [Fact]
        public void BuildsCanonicalLink()
        {
            var customer = new Customer {Id = 42};
            var link = LinkHelper.GetCanonicalLinkForModel(customer);
            Assert.NotNull(link);
            Assert.Equal("/customer/42", link.Href);
            Assert.Equal("self", link.Rel);
            Assert.Equal("application/vnd.customer", link.Type);
        }
        
        [Fact]
        public void BuildsCanonicalLinkWithDefaultUriTemplate()
        {
            var location = new Location {Id = 54};
            var link = LinkHelper.GetCanonicalLinkForModel(location);
            Assert.NotNull(link);
            Assert.Equal("/locations/54", link.Href);
            Assert.Equal("self", link.Rel);
            Assert.Equal("application/vnd.location", link.Type);
        }

        [Fact]
        public void GetsRootLinks()
        {
            var links = LinkHelper.GetRootLinks();
            var link = links.Single(l => l.GetHandlerType() == typeof (LocationsHandler));
            Assert.Equal("/locations", link.Href);
            Assert.Equal("locations", link.Rel);
            Assert.Equal("application/vnd.list.location", link.Type);
        }
    }

    public class Customer
    {
        public int Id { get; set; }
    }

    public class Location
    {
        public int Id { get; set; }
    }

    public class CustomerList
    {
        public IList<Customer> Items { get; set; }
    }

    [LinksFrom(typeof(Customer), "/customers/{Id}/orders", Rel = "customer.orders", Type = "application/vnd.list.order")]
    public class CustomerOrdersHandler
    {
        
    }

    [Canonical(typeof(Customer), "/customer/{Id}", Type = "application/vnd.customer")]
    public class CustomerHandler
    {
        
    }
    
    [UriTemplate("/locations/{Id}")]
    [Canonical(typeof(Location), Type = "application/vnd.location")]
    public class LocationHandler
    {
        
    }

    [UriTemplate("/customers/{Id}/location")]
    [LinksFrom(typeof(Customer), Rel = "customer.location", Type = "application/vnd.location")]
    public class CustomerLocationHandler
    {
        
    }

    [UriTemplate("/locations")]
    [Root(Rel = "locations", Type = "application/vnd.list.location")]
    public class LocationsHandler
    {
        
    }

    public class QueueMessageDto
    {
        public string Id { get; set; }
        public string Text { get; set; }
        public int DequeueCount { get; set; }
        public DateTimeOffset? ExpirationTime { get; set; }
        public DateTimeOffset? InsertionTime { get; set; }
        public DateTimeOffset? NextVisibleTime { get; set; }
        public string PopReceipt { get; set; }
        public long? SecondsToLive { get; set; }
        public long? SecondsVisiblityDelay { get; set; }
        public string QueueName { get; set; }
        public string AccountName { get; set; }
    }

    public class DequeueDto
    {
        public int RemainingCount { get; set; }
        public IList<QueueMessageDto> Messages { get; set; }
    }

    [UriTemplate("/{Name}/{Id}")]
    [LinksFrom(typeof (QueueMessageDto), "/api/{AccountName}/queues/{Name}/{Id}?pop={PopReceipt}", Rel = "delete")]
    public class DeleteMessage : IDeleteAsync
    {
        public async Task<Status> Delete()
        {
            return 200;
        }
    }
}