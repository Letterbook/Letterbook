---
title: Typesense Real-Time Search
---

Many popular Open Source products use [Algolia DocSearch](https://docsearch.algolia.com) to power their real-time search
features, however, it's a less appealing product for commercial products which is a paid service with a per request 
pricing model that made it difficult to determine what costs would be in the long run.

<a class="flex flex-col items-center my-8" href="https://typesense.org">
    <svg fill="none" height="56" viewBox="0 0 250 56" width="250" xmlns="http://www.w3.org/2000/svg">
        <g fill="#1035bc"><path d="m15.0736 15.8466c.1105.5522.1657 1.0859.1657 1.6013 0 .4785-.0552.9938-.1657 1.546l-7.01225-.0552v18.5521c0 1.546.71779 2.319 2.15335 2.319h4.1963c.2577.6258.3865 1.2516.3865 1.8773 0 .6258-.0368 1.0123-.1104 1.1595-1.6932.2209-3.4417.3313-5.24538.3313-3.57055 0-5.35583-1.5276-5.35583-4.5828v-19.6564l-3.920246.0552c-.1104293-.5522-.165644-1.0675-.165644-1.546 0-.5154.0552147-1.0491.165644-1.6013l3.920246.0552v-5.7975c0-.99387.14724-1.69325.44172-2.09816.29448-.44172.86503-.66258 1.71165-.66258h1.4908l.33129.33129v8.28225z"/><path d="m41.7915 16.1227-7.5644 25.8957c-1.3988 4.7485-2.8896 8.0982-4.4724 10.0491s-3.9571 2.9264-7.1227 2.9264c-1.6196 0-3.1104-.2393-4.4724-.7178-.1104-1.0307.1841-2.0246.8834-2.9816 1.1411.4049 2.3559.6073 3.6442.6073 1.9509 0 3.4417-.6625 4.4724-1.9877 1.0307-1.3251 1.9693-3.3865 2.816-6.184l.1656-.5522c-.9571-.0736-1.6932-.2945-2.2086-.6626-.4785-.3681-.8834-1.049-1.2147-2.0429l-7.7301-24.2945c1.1411-.4785 1.9509-.7178 2.4295-.7178 1.0675 0 1.7853.6442 2.1534 1.9325l4.3619 13.8589c.1473.4418.9939 3.3129 2.5399 8.6135.0736.2577.2577.3865.5521.3865l6.7362-24.4049c.4786-.1472 1.1043-.2208 1.8773-.2208.8099 0 1.4908.1104 2.043.3313z"/><path d="m52.4009 41.135v10.9325c0 .9938-.1472 1.6932-.4417 2.0981-.2945.4418-.8834.6626-1.7669.6626h-1.4908l-.3312-.3313v-38.4846l.3312-.3313h1.4356c.8835 0 1.4724.2392 1.7669.7178.3313.4417.4969 1.1779.4969 2.2086v.276c2.2086-2.4662 4.8405-3.6994 7.8957-3.6994 3.1289 0 5.4847 1.27 7.0675 3.8099 1.5828 2.503 2.3743 5.9816 2.3743 10.4356 0 2.1717-.2945 4.1226-.8835 5.8527-.5521 1.7301-1.3067 3.2025-2.2638 4.4172-.9202 1.1779-1.9877 2.0981-3.2024 2.7607-1.2148.6258-2.4663.9387-3.7546.9387-2.5399 0-4.951-.7546-7.2332-2.2638zm0-17.9448v14.1902c2.2454 1.6564 4.362 2.4846 6.3497 2.4846 1.9878 0 3.6258-.8834 4.9141-2.6503 1.2884-1.7668 1.9326-4.4356 1.9326-8.0061 0-1.7669-.1657-3.2945-.497-4.5828-.2945-1.3252-.6994-2.4111-1.2147-3.2577-.5153-.8834-1.1227-1.5276-1.8221-1.9325-.6626-.4417-1.3804-.6626-2.1534-.6626-1.4724 0-2.8711.3865-4.1963 1.1595-1.3251.773-2.4294 1.8589-3.3129 3.2577z"/><path d="m97.6973 30.6442h-17.1166c.1841 6.2576 2.5583 9.3865 7.1227 9.3865 2.5031 0 5.1718-.773 8.0061-2.319.8099.7361 1.3068 1.6748 1.4909 2.8159-3.0184 2.0614-6.405 3.092-10.1596 3.092-1.9141 0-3.5521-.3497-4.9141-1.049-1.3619-.7362-2.4846-1.7301-3.3681-2.9816-.8466-1.2884-1.4724-2.7976-1.8773-4.5277-.4049-1.73-.6073-3.6257-.6073-5.6871 0-2.0981.2392-4.0122.7178-5.7423.5153-1.7301 1.2515-3.2209 2.2085-4.4724.9571-1.2515 2.0982-2.227 3.4234-2.9264 1.3619-.6994 2.9079-1.0491 4.638-1.0491 1.6932 0 3.2025.3129 4.5276.9387 1.362.589 2.4847 1.4172 3.3681 2.4847.9203 1.0306 1.6196 2.2822 2.0982 3.7546.4785 1.4355.7178 2.9816.7178 4.638 0 .6626-.0369 1.3067-.1105 1.9325-.0368.589-.092 1.1595-.1656 1.7117zm-17.1166-3.1473h13.2516v-.7178c0-2.5398-.5338-4.5828-1.6013-6.1288s-2.6687-2.319-4.8037-2.319c-2.0981 0-3.7362.8282-4.9141 2.4847-1.1411 1.6564-1.7852 3.8834-1.9325 6.6809z"/><path d="m103.381 41.0245c.036-.8098.257-1.6932.662-2.6503.442-.9938.939-1.7668 1.491-2.319 2.908 1.5828 5.466 2.3743 7.675 2.3743 1.214 0 2.19-.2393 2.926-.7178.773-.4786 1.16-1.1227 1.16-1.9326 0-1.2883-.994-2.319-2.982-3.092l-3.092-1.1595c-4.638-1.6932-6.957-4.3988-6.957-8.1166 0-1.3251.239-2.503.718-3.5337.515-1.0675 1.214-1.9693 2.098-2.7055.92-.773 2.006-1.362 3.258-1.7669 1.251-.4049 2.65-.6074 4.196-.6074.699 0 1.472.0553 2.319.1657.883.1104 1.767.2761 2.65.4969.884.1841 1.73.4049 2.54.6626s1.509.5337 2.098.8282c0 .9203-.184 1.8773-.552 2.8712s-.865 1.73-1.491 2.2086c-2.908-1.2884-5.429-1.9325-7.564-1.9325-.957 0-1.712.2392-2.264.7178-.552.4417-.828 1.0306-.828 1.7668 0 1.1411.92 2.043 2.761 2.7055l3.368 1.2148c2.429.8466 4.233 2.0061 5.411 3.4785s1.767 3.184 1.767 5.135c0 2.6135-.976 4.7116-2.927 6.2944-1.951 1.5461-4.748 2.3191-8.392 2.3191-3.571 0-6.921-.9019-10.049-2.7056z"/><path d="m151.772 31.5828h-15.239c.111 2.0246.571 3.6258 1.38 4.8037.847 1.1411 2.301 1.7117 4.362 1.7117 2.135 0 4.583-.6258 7.344-1.8773 1.067 1.1042 1.748 2.5582 2.043 4.3619-2.945 2.0982-6.479 3.1473-10.601 3.1473-3.902 0-6.865-1.1964-8.89-3.589-1.988-2.4294-2.981-6.0184-2.981-10.7669 0-2.2086.257-4.1963.773-5.9632.515-1.8036 1.269-3.3312 2.263-4.5828.994-1.2883 2.209-2.2822 3.644-2.9816 1.436-.6994 3.074-1.0491 4.915-1.0491 1.877 0 3.533.2945 4.969.8835 1.436.5521 2.65 1.3619 3.644 2.4294.994 1.0307 1.73 2.2638 2.209 3.6994.515 1.4356.773 3 .773 4.6933 0 .9202-.056 1.8036-.166 2.6503-.11.8098-.258 1.6196-.442 2.4294zm-10.656-11.5951c-2.871 0-4.417 2.1718-4.638 6.5154h9.165v-.6626c0-1.7669-.368-3.1841-1.104-4.2515-.736-1.0675-1.877-1.6013-3.423-1.6013z"/><path d="m182.033 24.5153v12.0368c0 2.3559.386 4.1043 1.159 5.2454-1.178 1.0307-2.595 1.5461-4.251 1.5461-1.583 0-2.669-.3497-3.258-1.0491-.589-.7362-.884-1.8773-.884-3.4233v-12.8651c0-1.6564-.202-2.8159-.607-3.4785s-1.159-.9939-2.264-.9939c-1.951 0-3.773.8835-5.466 2.6504v18.773c-.552.1104-1.141.184-1.767.2208-.589.0368-1.196.0552-1.822.0552s-1.251-.0184-1.877-.0552c-.589-.0368-1.16-.1104-1.712-.2208v-27.3313l.331-.3865h2.761c2.062 0 3.35 1.1043 3.865 3.3128 2.687-2.319 5.356-3.4785 8.006-3.4785 2.651 0 4.602.8651 5.853 2.5951 1.288 1.6933 1.933 3.9755 1.933 6.8466z"/><path d="m187.825 41.0245c.036-.8098.257-1.6932.662-2.6503.442-.9938.939-1.7668 1.491-2.319 2.908 1.5828 5.466 2.3743 7.675 2.3743 1.214 0 2.19-.2393 2.926-.7178.773-.4786 1.16-1.1227 1.16-1.9326 0-1.2883-.994-2.319-2.982-3.092l-3.092-1.1595c-4.638-1.6932-6.957-4.3988-6.957-8.1166 0-1.3251.239-2.503.718-3.5337.515-1.0675 1.214-1.9693 2.098-2.7055.92-.773 2.006-1.362 3.258-1.7669 1.251-.4049 2.65-.6074 4.196-.6074.699 0 1.472.0553 2.319.1657.883.1104 1.767.2761 2.65.4969.884.1841 1.73.4049 2.54.6626s1.509.5337 2.098.8282c0 .9203-.184 1.8773-.552 2.8712s-.865 1.73-1.491 2.2086c-2.908-1.2884-5.429-1.9325-7.564-1.9325-.957 0-1.712.2392-2.264.7178-.552.4417-.828 1.0306-.828 1.7668 0 1.1411.92 2.043 2.761 2.7055l3.368 1.2148c2.429.8466 4.233 2.0061 5.411 3.4785s1.767 3.184 1.767 5.135c0 2.6135-.976 4.7116-2.927 6.2944-1.951 1.5461-4.748 2.3191-8.392 2.3191-3.571 0-6.921-.9019-10.049-2.7056z"/><path d="m236.216 31.5828h-15.239c.111 2.0246.571 3.6258 1.38 4.8037.847 1.1411 2.301 1.7117 4.362 1.7117 2.135 0 4.583-.6258 7.344-1.8773 1.067 1.1042 1.748 2.5582 2.043 4.3619-2.945 2.0982-6.479 3.1473-10.601 3.1473-3.902 0-6.865-1.1964-8.89-3.589-1.988-2.4294-2.981-6.0184-2.981-10.7669 0-2.2086.257-4.1963.773-5.9632.515-1.8036 1.269-3.3312 2.263-4.5828.994-1.2883 2.209-2.2822 3.645-2.9816 1.435-.6994 3.073-1.0491 4.914-1.0491 1.877 0 3.533.2945 4.969.8835 1.436.5521 2.65 1.3619 3.644 2.4294.994 1.0307 1.73 2.2638 2.209 3.6994.515 1.4356.773 3 .773 4.6933 0 .9202-.055 1.8036-.166 2.6503-.11.8098-.258 1.6196-.442 2.4294zm-10.656-11.5951c-2.871 0-4.417 2.1718-4.638 6.5154h9.166v-.6626c0-1.7669-.369-3.1841-1.105-4.2515-.736-1.0675-1.877-1.6013-3.423-1.6013z"/><path d="m244.777 50.6871v-50.521455c.552-.1104299 1.178-.165645 1.878-.165645.736 0 1.417.0552151 2.042.165645v50.521455c-.625.1104-1.306.1657-2.042.1657-.7 0-1.326-.0553-1.878-.1657z"/></g>
    </svg>
</a>

We discovered [Typesense](https://typesense.org) as an appealing alternative which offers [simple cost-effective cloud hosting](https://cloud.typesense.org)
but even better, they also have an easy to use open source option for self-hosting or evaluation. Given its effortless integration, 
simplicity-focus and end-user UX, it quickly became our preferred way to navigate [docs.servicestack.net](https://docs.servicestack.net). 

To make it easier to adopt Typesense's amazing OSS Search product we've documented the approach we use to create and 
deploy an index of our site automatically using GitHub Actions that you could also utilize in your Razor Press websites.

Documentation search is a common use case which Typesense caters for with their 
[typesense-docsearch-scraper](https://github.com/typesense/typesense-docsearch-scraper) - a utility designed to easily 
scrape a website and post the results to a Typesense server to create a fast searchable index.

## Self hosting option

We recommend using running their [easy to use Docker image](https://hub.docker.com/r/typesense/typesense/) to run
an instance of their Typesense server, which you can run in a **t2.small** AWS EC2 instance or in a 
[Hetzner Cloud](https://www.hetzner.com/cloud) VM for a more cost effective option.

Trying it locally, we used the following commands to spin up a local Typesense server ready to scrape out docs site.

```sh
mkdir /tmp/typesense-data
docker run -p 8108:8108 -v/tmp/data:/data typesense/typesense:0.21.0 \
    --data-dir /data --api-key=<temp-admin-api-key> --enable-cors
```

To check that the server is running, we can open a browser at `/health` and we get back 200 OK with `ok: true`.

The Typesense server has a [REST API](https://typesense.org/docs/0.21.0/api) which can be used to manage the indexes you create. 
Or if you use their cloud offering, you can use their web dashboard to monitor and manage your index data.

## Populating the index

With your local server is running, you can scrape your docs site using the 
[typesense-docsearch-scraper](https://github.com/typesense/typesense-docsearch-scraper). 
This needs some configuration to tell the scraper:

- Where the Typesense server is
- How to authenticate with the Typesense server
- Where the docs website is
- Rules for the scraper to follow extracting information from the docs website

These [pieces of configuration come from 2 sources](https://github.com/ServiceStack/docs/tree/master/search-server/typesense-scraper). 
A [.env](https://github.com/ServiceStack/docs/blob/master/search-server/typesense-scraper/typesense-scraper.env) file 
related to the Typesense server information and
a [.json](https://github.com/ServiceStack/docs/blob/master/search-server/typesense-scraper/typesense-scraper-config.json)
file related to what site will be getting scraped.

With a Typesense running locally on port **8108**, we configure the `.env` file with the following information:

```
TYPESENSE_API_KEY=${TYPESENSE_API_KEY}
TYPESENSE_HOST=localhost
TYPESENSE_PORT=8108
TYPESENSE_PROTOCOL=http
```

Next, we have to configure the `.json` config for the scraper. 
The **typesense-docsearch-scraper** has [an example of this](https://github.com/typesense/typesense-docsearch-scraper/blob/master/configs/public/typesense_docs.json)
config in their repository.

The default selectors will need to match the your websites HTML, which for Razor Press sites can start with the configuration,
updated with your website domains:

```json
{
  "index_name": "typesense_docs",
  "allowed_domains": ["docs.servicestack.net"],
  "start_urls": [
    {
      "url": "https://docs.servicestack.net/"
    }
  ],
  "selectors": {
    "default": {
      "lvl0": "h1",
      "lvl1": ".content h2",
      "lvl2": ".content h3",
      "lvl3": ".content h4",
      "lvl4": ".content h5",
      "text": ".content p, .content ul li, .content table tbody tr"
    }
  },
  "scrape_start_urls": false,
  "strip_chars": " .,;:#"
}
```

With both the configuration files ready to use, we can run the scraper itself. The scraper is also available using the 
docker image `typesense/docsearch-scraper` which we can pass our configuration to, using the following command:

```sh
docker run -it --env-file typesense-scraper.env \
    -e "CONFIG=$(cat typesense-scraper-config.json | jq -r tostring)" \
    typesense/docsearch-scraper
```

Here `-i` is used to reference a local `--env-file` and use `cat` and `jq` used to populate the `CONFIG` environment variable 
with the `.json` config file.

## Docker networking

We had a slight issue here since the scraper itself is running in Docker via WSL and `localhost` doesn't resolve to our 
host machine to find the Typesense server also running in Docker.
Instead we need to point the scraper to the Typesense server using the Docker local IP address space of `172.17.0.0/16` 
for it to resolve without additional configuration.

We can see in the output of the Typesense server that it is running using `172.17.0.2`. We can swap the `localhost` 
with this IP address after which we see the communication between the servers flowing:

```
DEBUG:typesense.api_call:Making post /collections/typesense_docs_1635392168/documents/import
DEBUG:typesense.api_call:Try 1 to node 172.17.0.2:8108 -- healthy? True
DEBUG:urllib3.connectionpool:Starting new HTTP connection (1): 172.17.0.2:8108
DEBUG:urllib3.connectionpool:http://172.17.0.2:8108 "POST /collections/typesense_docs_1635392168/documents/import HTTP/1.1" 200 None
DEBUG:typesense.api_call:172.17.0.2:8108 is healthy. Status code: 200
> DocSearch: https://docs.servicestack.net/azure 22 records)
DEBUG:typesense.api_call:Making post /collections/typesense_docs_1635392168/documents/import
DEBUG:typesense.api_call:Try 1 to node 172.17.0.2:8108 -- healthy? True
DEBUG:urllib3.connectionpool:Starting new HTTP connection (1): 172.17.0.2:8108
DEBUG:urllib3.connectionpool:http://172.17.0.2:8108 "POST /collections/typesense_docs_1635392168/documents/import HTTP/1.1" 200 None
```

The scraper crawls the docs site following all the links in the same domain to get a full picture of all the content of our docs site.
This takes a minute or so, and in the end we can see in the Typesense sever output that we now have **committed_index: 443**.

```
_index: 443, applying_index: 0, pending_index: 0, disk_index: 443, pending_queue_size: 0, local_sequence: 44671
I20211028 03:39:40.402626   328 raft_server.h:58] Peer refresh succeeded!
```

## Searching content

After you have a Typesense server with an index full of content, you'll want to be able to use it to search your docs site.
You can query the index using `curl` which needs to known 3 key pieces of information:

- Collection name, eg `typesense_docs`
- Query term, `?q=test`
- What to query, `&query_by=content`

```sh
curl -H 'x-typesense-api-key: <apikey>' \
    'http://localhost:8108/collections/typesense_docs/documents/search?q=test&query_by=content'
```

The collection name and `query_by` come from how the scraper was configured. The scraper was posting data to the
`typesense_docs` collection and populating various fields, eg `content`.

Which as it returns JSON can be easily queried in JavaScript using **fetch**:

```js
fetch('http://localhost:8108/collections/typesense_docs/documents/search?q='
    + encodeURIComponent(query) + '&query_by=content', {
    headers: {
        // Search only API key for Typesense.
        'x-typesense-api-key': 'TYPESENSE_SEARCH_ONLY_API_KEY'
    }
})
```

In the above we have also used a different name for the API key token, this is important since the `--api-key` specified to the running Typesense server is the admin API key. You don't want to expose this to a browser client since they will have the ability to create,update and delete your collections or documents.

Instead we want to generate a "Search only" API key that is safe to share on a browser client. This can be done using the Admin API key and the following REST API call to the Typesense server.

```bash
curl 'http://localhost:8108/keys' -X POST \
  -H "X-TYPESENSE-API-KEY: ${TYPESENSE_API_KEY}" \
  -H 'Content-Type: application/json' \
  -d '{"description": "Search only","actions": ["documents:search"],"collections":["*"]}'
```

Now we can share this generated key safely to be used with any of our browser clients.

## Keeping the index updated

A problem that becomes apparent when running the scraper is that it increases the size of the index since it currently 
doesn't detect and update existing documents. It wasn't clear if this is possible to configure from the current scraper, 
but we needed a way to achieve the following goals:

- Update the search index automatically soon after docs have been changed
- Don't let the index grow too big to avoid manual intervention
- Have high uptime so documentation search is always available

Typesense server itself performs extremely well, so a full update from the scraper doesn't generate an amount of load. 
However, every additional scrape uses additional disk space and memory that will eventually require periodically 
resetting and repopulating the index.

One option is to switch to a new collection everytime the documentation is updated and delete the old collection,
adopting a workflow that looks something like: 

1. Docs are updated
2. Publish updated docs
3. Create new collection, store new and old names
4. Scrape updated docs
5. Update client with new collection
6. Delete old collection

However this would require orchestration across a number of GitHub Action workflows which we anticipated would be 
fragile and non-deterministic as to how long it will take to scrape, update, and deploy our changes.

## Read-only Docker container

The approach we ended up adopting was to develop and deploy read only Typesense Docker images containing an immutable copy 
of the index data in it as part of the GitHub Action deployments.

In the case of Typesense, when it starts up, it reads from its `data` directory from disk to populate the index in 
memory and since our index is small and only updates when our documentation is updated, we can simplify the management 
of the index data by **baking it into the docker image**.

This has several key advantages.

- Disaster recovery doesn't need any additional data management.
- Shipping an updated index is a normal ECS deployment.
- Zero down time deployments.
- Index is of a fixed size once deployed.

## Typesense Performance

Search on our documentation site is a very light workload for Typesense. Running as an ECS service on a 2 vCPU instance, 
the service struggled to get close to **1%** whilst serving constant typeahead searching.

![](https://servicestack.net/img/posts/typesense/typesense-cpu-utilization.png)

Since our docs site index is small (500 pages), the memory footprint is also tiny and stable at **~50MB** or **~10%** of the the 
service's soft memory limit.

![](https://servicestack.net/img/posts/typesense/typesense-memory-utilization.png)

This means we will be able to host this using a single EC2 instance among various other or the ServiceStack hosted 
example applications and use the same deployment patterns we've shared in our
[GitHub Actions templates](https://docs.servicestack.net/mix-github-actions-aws-ecs).

[![](https://raw.githubusercontent.com/ServiceStack/docs/master/docs/images/mix/cloudcraft-host-digram-release-ecr-aws.png)](https://docs.servicestack.net/mix-github-actions-aws-ecs)

Whilst this approach of shipping an index along with the Docker image isn't practical for large or 'living' indexes, 
many small to medium-sized documentation sites would likely benefit from the simplified approach of deploying readonly
Docker images.

## GitHub Actions Workflow

To create our own Docker image for our search server we need to perform the following tasks in our GitHub Action:

1. Run a local Typesense server in the GitHub Action using Docker
2. Scrape our hosted docs populating the local Typesense server
3. Copy the `data` folder of our local Typesense server during `docker build`

Which is done with:

```sh
mkdir -p ${GITHUB_WORKSPACE}/typesense-data
cp ./search-server/typesense-server/Dockerfile ${GITHUB_WORKSPACE}/typesense-data/Dockerfile
cp ./search-server/typesense-scraper/typesense-scraper-config.json typesense-scraper-config.json
envsubst < "./search-server/typesense-scraper/typesense-scraper.env" > "typesense-scraper-updated.env"
docker run -d -p 8108:8108 -v ${GITHUB_WORKSPACE}/typesense-data/data:/data \
    typesense/typesense:0.21.0 --data-dir /data --api-key=${TYPESENSE_API_KEY} --enable-cors &
# wait for typesense initialization
sleep 5
docker run -i --env-file typesense-scraper-updated.env \
    -e "CONFIG=$(cat typesense-scraper-config.json | jq -r tostring)" typesense/docsearch-scraper
```

Our `Dockerfile` then takes this data from the `data` folder during build.

```Dockerfile
FROM typesense/typesense:0.21.0

COPY ./data /data
```

To avoid updating our search client between updates we also want to use the same **search-only API Key** everytime 
a new server is created. This can be achieved by specifying `value` in the `POST` command sent to the local Typesense server:

```sh
curl 'http://172.17.0.2:8108/keys' -X POST \
  -H "X-TYPESENSE-API-KEY: ${TYPESENSE_API_KEY}" \
  -H 'Content-Type: application/json' \
  -d '{"value":<search-api-key>,"description":"Search only","actions":["documents:search"],"collections":["*"]}'
```

If you're interested in adopting a similar approach you can find the whole GitHub Action workflow in our 
[search-index-update.yml](https://github.com/ServiceStack/docs/blob/master/.github/workflows/search-index-update.yml) 
workflow.

## Search UI Dialog

After docs are indexed the only thing left to do is display the results. We set out to create a comparable UX to
Algolia's doc search dialog which we've implemented in the
[Typesense.mjs](https://github.com/ServiceStack/docs.servicestack.net/blob/main/Letterbook.Docs/wwwroot/mjs/components/Typesense.mjs)
Vue component which you can register as a global component in your 
[app.mjs](https://github.com/ServiceStack/docs.servicestack.net/blob/main/Letterbook.Docs/wwwroot/mjs/app.mjs):


```js
import Typesense from "./components/Typesense.mjs"

const Components = {
    //...
    Typesense,
}
```

Which renders as a **Search Button** that we've added next to our **Dark Mode Toggle** button in our 
[Header.cshtml](https://github.com/ServiceStack/docs.servicestack.net/blob/main/Letterbook.Docs/Pages/Shared/Header.cshtml):

```html
<div class="hidden sm:ml-6 sm:flex sm:items-center">
    <typesense></typesense>
    <dark-mode-toggle class="ml-2 w-10"></dark-mode-toggle>
</div>
```

![](https://servicestack.net/img/posts/typesense/typesense-header.png)

The button also encapsulates the dialog component which uses Typesense REST API to query to our typesense instance:

```js
fetch('https://search.docs.servicestack.net/collections/typesense_docs/documents/search?q='
  + encodeURIComponent(query.value)
  + '&query_by=content,hierarchy.lvl0,hierarchy.lvl1,hierarchy.lvl2,hierarchy.lvl3&group_by=hierarchy.lvl0', {
    headers: {
      // Search only API key for Typesense.
      'x-typesense-api-key': 'TYPESENSE_SEARCH_ONLY_API_KEY'
    }
})
```

This instructs Typesense to search through each documents content and h1-3 headings, grouping results by its page title.
Refer to the [Typesense API Search Reference](https://typesense.org/docs/0.21.0/api/documents.html#search) to learn how
to further fine-tune search results for your use-case.

## Search Results

![](https://servicestack.net/img/posts/typesense/typesense-dart.gif)

The results are **excellent**, [see for yourself](https://docs.servicestack.net) by using the search at the top right or 
using `Ctrl+K` shortcut key on [docs.servicestack.net](https://docs.servicestack.net).

It also does a great job handling typos and has quickly become the fastest way to navigate our extensive documentation
that we hope also serves useful for implementing Typesense real-time search in your own documentation websites.