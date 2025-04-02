![IAB Tech Lab](https://drive.google.com/uc?id=10yoBoG5uRETSXRrnJPUDuONujvADrSG1)

# OpenRTB Ad Management API v1.1 Specification

**OpenRTB 3.0 and AdCOM Companion Specification**

January 23, 2023


**Executive Summary**

The IAB Tech Lab Ad Management API specification is part of the [OpenMedia specification suite](iabtechlab.com/openmedia). 

Ad management occurs when a buyer (or a representative party) submits creatives for creative approval, supply platforms approve or disapprove of those creatives, and buyers receive feedback accordingly. Before the publication of this technical standard, supply platforms have relied on proprietary methods and tools for ad management, or none at all. 

This process is important for a few reasons: to ensure creatives comply with content guidelines, are malware-free, and function correctly. Creative submission is also a technical necessity for certain emerging formats such as digital out-of-home (DOOH) and programmatic TV. In an ad quality review process, analysts or automated processes check the ad's landing page, tracking tags, text, the creative itself, and more. The exact nature of the review process is up to each supply platform. The results of these checks are made available for buying platforms to consume and modify bidding behavior accordingly.

The Programmatic Supply Chain Working Group has identified a need to standardize the creative submission and ad management process to reduce pain points for buyers and sellers in the digital advertising industry. Using a standard approach allows for ease of integration between buy-side and supply-side platforms. Supply platforms using the standardized Ad Management API gain increased control over the creatives that serve on their platforms. Both supply platforms and demand platforms benefit from increased bidding efficiency, as demand platforms can avoid invalid bids and notify customers of defects with their ads. This can unblock revenue that would otherwise be received if buyers knew that they must make adjustments. Submission of ads in advance also benefits all parties as it reduces approval delays that may exist with current workflows. 

The Ad Management API specification support all major scenarios known at time of publication for both bidding and markup delivery. For bidding, this refers to whether supply platforms permit ads to serve by default ("permissive bidding") or require explicit approval before serving ("restrictive bidding"). For markup delivery, this refers to whether the markup is included in each bid or whether bids refer to pre-uploaded markup by ID.

**About IAB Tech Lab**

The IAB Technology Laboratory is a nonprofit research and development consortium charged with producing and helping companies implement global industry technical standards and solutions. The goal of the Tech Lab is to reduce friction associated with the digital advertising and marketing supply chain while contributing to the safe growth of an industry. The IAB Tech Lab spearheads the development of technical standards, creates and maintains a code library to assist in rapid, cost-effective implementation of IAB standards, and establishes a test platform for companies to evaluate the compatibility of their technology solutions with IAB standards, which for 18 years have been the foundation for interoperability and profitable growth in the digital advertising supply chain.

Learn more about IAB Tech Lab here: https://www.iabtechlab.com/

**Original Author of the Ad Management API Specification Proposal**

Ian Trider, VP, RTB Platform Operations, Basis and IAB Tech Lab Commit Group Member

**IAB Tech Lab Programmatic Supply Chain Commit Group Members**
[https://iabtechlab.com/working-groups/programmatic-supply-chain-commit-group/](https://iabtechlab.com/working-groups/programmatic-supply-chain-commit-group/)

**IAB Tech Lab Programmatic Supply Chain Working Group Members**

[https://iabtechlab.com/working-groups/programmatic-supply-chain-working-group/](https://iabtechlab.com/working-groups/programmatic-supply-chain-working-group/) 

**IAB Tech Lab Contact**

For more information, or to get involved, please email support@iabtechlab.com.

**Contributors and Technical Governance**

Programmatic Supply Chain Working Group members provide contributions to this repository. Participants in the Programmatic Supply Working group must be members of IAB Tech Lab. Technical Governance and code commits for the project are provided by the IAB Tech Lab Programmatic Supply Chain Commit Group.

Learn more about how to submit changes in our working group: [So, You'd Like to Propose a Change...](https://iabtechlab.com/blog/so-youd-like-to-propose-a-change-to-openrtb-adcom/)

**License**
OpenRTB Specification the IAB Tech Lab is licensed under a Creative Commons Attribution 3.0 License. To view a copy of this license, visit creativecommons.org/licenses/by/3.0/ or write to Creative Commons, 171 Second Street, Suite 300, San Francisco, CA 94105, USA.

**Disclaimer**
THE STANDARDS, THE SPECIFICATIONS, THE MEASUREMENT GUIDELINES, AND ANY OTHER MATERIALS OR SERVICES PROVIDED TO OR USED BY YOU HEREUNDER (THE “PRODUCTS AND SERVICES”) ARE PROVIDED “AS IS” AND “AS AVAILABLE,” AND IAB TECHNOLOGY LABORATORY, INC. (“TECH LAB”) MAKES NO WARRANTY WITH RESPECT TO THE SAME AND HEREBY DISCLAIMS ANY AND ALL EXPRESS, IMPLIED, OR STATUTORY WARRANTIES, INCLUDING, WITHOUT LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AVAILABILITY, ERROR-FREE OR UNINTERRUPTED OPERATION, AND ANY WARRANTIES ARISING FROM A COURSE OF DEALING, COURSE OF PERFORMANCE, OR USAGE OF TRADE. TO THE EXTENT THAT TECH LAB MAY NOT AS A MATTER OF APPLICABLE LAW DISCLAIM ANY IMPLIED WARRANTY, THE SCOPE AND DURATION OF SUCH WARRANTY WILL BE THE MINIMUM PERMITTED UNDER SUCH LAW. THE PRODUCTS AND SERVICES DO NOT CONSTITUTE BUSINESS OR LEGAL ADVICE. TECH LAB DOES NOT WARRANT THAT THE PRODUCTS AND SERVICES PROVIDED TO OR USED BY YOU HEREUNDER SHALL CAUSE YOU AND/OR YOUR PRODUCTS OR SERVICES TO BE IN COMPLIANCE WITH ANY APPLICABLE LAWS, REGULATIONS, OR SELF-REGULATORY FRAMEWORKS, AND YOU ARE SOLELY RESPONSIBLE FOR COMPLIANCE WITH THE SAME, INCLUDING, BUT NOT LIMITED TO, DATA PROTECTION LAWS, SUCH AS THE PERSONAL INFORMATION PROTECTION AND ELECTRONIC DOCUMENTS ACT (CANADA), THE DATA PROTECTION DIRECTIVE (EU), THE E-PRIVACY DIRECTIVE (EU), THE GENERAL DATA PROTECTION REGULATION (EU), AND THE E-PRIVACY REGULATION (EU) AS AND WHEN THEY BECOME EFFECTIVE.

# Table of Contents

- [Introduction and Background](#intro)
  - [Dependencies](#dependencies)
  - [Bidding Scenarios](#biddingscenarios)
    - [Permissive Bidding](#permissivebidding)
    - [Restrictive Bidding](#restrictivebidding)
  - [Markup Delivery Methods](#markupdeliverymethods)
    - [Ad Markup In Bid Response](#admarkupinbidresponse)
    - [Exchange-hosted Ad Markup](#exchangehostedadmarkup)
  - [Rationale For This Specification](#rationale)
    - [Restrictive Bidding, Supplier-hosted Ad Markup](#restrictivebiddingexchangehostedadmarkup)
    - [Restrictive Bidding, Ad Markup In Bid Response](#restrictivebiddingadmarkupinbidresponse)
    - [Permissive Bidding, Ad Markup In Bid Response](#permissivebiddingadmarkupinbidresponse)
    - [Emerging Formats](#emergingformats)
- [API Conventions](#apiconventions)
  - [Status Codes](#statuscodes)
  - [Sparse Fieldsets](#sparsefieldsets)
  - [Dates And Times](#datesandtimes)
- [Endpoints](#endpoints)
- [Authentication](#authentication)
- [Resource Representations](#resourcerepresentations)
  - [Collection Of Ads](#collectionofads)
  - [Ad](#ad)
  - [Audit](#audit)
- [Webhooks](#webhooks)
  - [Registration and Authentication](#webhookauthentication)
  - [Webhook Calls](#webhookcalls)
- [Expiration](#expiration)
  - [Re-activating Expired Ads](#reactivate)
- [Requesting Re-Audit](#reaudit)
- [Substitution Macros](#substitutionmacros)
- [Typical Synchronization Flow](#typicalsynchronizationflow)
- [Appendix A: Integration Checklist](#appendixa_integrationchecklist)
- [Appendix B: Examples](#appendixb_examples)
  - [Minimal Implementation](#minimalimplementation)
    - [Bidder Ad Submission](#bidderadsubmission1)
    - [Bidder Receives A Webhook Call From Exchange](#bidderreceivesawebhookcallfromexchange1)
    - [Bidder Polls For Updates](#bidderpollsforupdates1)
  - [Typical Implementation](#typicalimplementation)
    - [Bidder Ad Submission](#bidderadsubmission2)
    - [Bidder Polls For Updates](#bidderpollsforupdates2)
    - [Bidder Re-activates an Ad](#bidderreactivates)
- [Appendix C: API Pagination](#pagination)
- [Appendix D: Resources](#appendixd_resources)
- [Appendix E: Change Log](#appendixe_changelog)
- [Appendix F: Errata](#appendixf_errata)



# Introduction and Background <a name="intro"></a>

The purpose of this specification is to provide a standardized means for demand and supply partners to communicate with each other regarding ads that will be used in bidding. The specification provides for a RESTful API to be implemented by supply partners.

Exchanges have vastly different policies with regards to creatives, and this specification makes no attempt to enforce any set of business rules. Rather, such rules are a matter for bidders and exchanges to communicate with each other as a part of integration. The terms "required", "recommended", and "optional" in this specification refer only to technical compliance, not business policy. Likewise, "must" and "will" refer to requirements for a technically compliant implementation.

Wherever possible, this specification makes reference to OpenRTB as it is assumed this is the standard "common language" of bidding that will be used between exchanges and bidders.  While the most common expected use case is that of a DSP interacting with an ad exchange, the OpenRTB Ad Management API is not limited to this purpose and can be used to transmit ad and audit information between partners in more complex supply chains. Wherever the term "bidder" is used, this should be understood to mean more generically "demand partner", and wherever the term "exchange" is used, this should be understood to mean more generically "supply partner." For example, an ad network may have a server-to-server integration with an ad exchange to source demand. In this case, the ad network is the supply partner and the exchange is the demand partner.

## Dependencies <a name="dependencies"></a>

This specification makes use of [AdCOM 1.x](https://github.com/InteractiveAdvertisingBureau/AdCOM) for the definition of the Ad object and its children. 

This specification is not inherently constrained to partners who use OpenRTB during bidding. In fact, this specification is also suitable for management of ads used in non-biddable transactions as well. **While this specification is released as a part of the OpenRTB 3.0 framework, implementation of OpenRTB 3.0 is not required to use this specification.** It can easily be used alongside OpenRTB 2.x bidding implementations.

**NOTE:** Out of convenience, this specification will refer to some shorthand terms for expected common scenarios. These are outlined in the following two sections.

## Bidding Scenarios <a name="biddingscenarios"></a>

### Permissive Bidding <a name="permissivebidding"></a>

In a permissive bidding scenario, the exchange allows a new ad to win impressions until (and if) a point in time occurs at which the ad is deemed to be disapproved. Bidders are expected to stop bidding with disapproved ads, and exchange may discard bid responses using such a creative.

### Restrictive Bidding <a name="restrictivebidding"><a/>

In a restrictive bidding scenario, the exchange does not allow new ads to win impressions until (and if) a point in time occurs at which the ad is deemed to be approved. Exchanges may discard bids made using such a ad until approval.

An exchange using restrictive bidding may choose to allow new ads to enter its audit queue by simply having the bidder start bidding with such an ad ("submission via bidding"), however bids may be discarded by the exchange until it is approved. Submission via bidding allows bidders to participate in auctions even if they have not implemented support for this API, however optimal performance is ensured by doing so.

## Markup Delivery Methods <a name="markupdeliverymethods"></a>

### Ad Markup In Bid Response <a name="admarkupinbidresponse"></a>

In this scenario, the bidder is expected to include the markup/URL for an ad (along with its ID) in each bid response.

The ad submitted by the bidder using this API is expected to be a faithful example of creative markup that behaves in a way that is representative of markup that will be used during bidding. It is expected that the exact markup will vary (for example, there will be varying components for an impression tracking URL, cachebusters, click URL, product IDs and images in dynamic creative, etc.). It is up to exchanges to communicate which deviations used during bidding will be deemed to constitute a material change to the creative (and thus may trigger a change in audit status).

### Exchange-hosted Ad Markup <a name="exchangehostedadmarkup"></a>

In this scenario, the exchange holds the ad markup. During bidding, the bidder makes reference to the markup on file using the "mid" field in the Bid object in the bid response (OpenRTB v3.x) or similar (non-OpenRTB or previous versions) and does not include the actual markup. The exchange provides a means for the bidder to specify custom macros in the markup for which values will be provided at bid time, so that variable components of the markup (impression tracking URLs, cachebusters, click URLs, etc.) may be substituted.

Note that "ad markup" refers to merely the contents of the adm field (for display ads) or similar (e.g. VAST); typically, such markup will contain references to third-party servers for assets, etc. 

**NOTE:** It is expected that exchanges will make clear to their demand partners as a matter of exchange policy whether permissive or restrictive bidding in place and whether bidders should return markup in bid responses or whether exchange-hosted ad markup applies.

## Rationale For This Specification <a name="rationale"></a>

There are a number of reasons why this specification was created. Consider the following sets of scenarios that exist for exchanges:

### Restrictive Bidding, Exchange-hosted Ad Markup <a name="restrictivebiddingexchangehostedadmarkup"></a>

In this scenario, there is an absolute need for an ad management API. This need is currently being filled by proprietary APIs, and DSPs must implement multiple proprietary specs to do business.

### Restrictive Bidding, Ad Markup In Bid Response <a name="restrictivebiddingadmarkupinbidresponse"></a>

In this scenario, when exchanges use "submission via bidding" there is an artificial delay before ads can start to spend -- they are often quarantined and bids for a given ad ID are disallowed from bidding until they have been reviewed by the exchange. The result is that money is "left on the table" while waiting for the audit to clear, campaigns do not begin on time, and (if the exchange and/or bidder is unable or unwilling to implement multiple bids in bid responses) bids are submitted that will be discarded, resulting in opportunity loss as the bidder may have had other viable bids for ads that are already approved. As a result, the rate of discarded bids can grow to be extremely high resulting in significant revenue loss.

The presence of a proprietary ad management API avoids the pitfalls of submission via bidding, but requires costly engineering resources, requiring demand partners to maintain multiple implementations for each exchange.

### Permissive Bidding, Ad Markup In Bid Response <a name="permissivebiddingadmarkupinbidresponse"></a>

This pattern is the most common among exchanges today. However, there are still problems that an ad management API helps rectify. It is not unusual for an exchange to block certain ad IDs platform-wide as a violation of their ad quality policy or due to technical reasons. This information is not communicated to the bidder, which continues to bid with an ad that will not be accepted.  If the exchange and/or bidder is unable or unwilling to implement multiple bids in bid responses, there is a great deal of potential revenue that is lost due to bids being filtered. This is particularly noteworthy with video, where technical problems with ads occur commonly, and as a result, exchanges implement blocks on defective ads. An ad management API makes it possible for the exchange to provide feedback to the bidder which it can incorporate to modify its bidding (e.g. discontinue bidding with ad IDs that the exchange has deemed to be unacceptable). 

### Emerging Formats <a name="emergingformats"></a>

New formats are emerging that are beginning to be transactable via real-time bidding. Particularly, digital out-of-home and programmatic TV are here or on the near horizon. These new formats often have stringent ad approval requirements that necessitate an API.

# API Conventions <a name="apiconventions"></a>

This API adheres to many of the conventions of RESTful APIs. The base protocol used for communication is HTTPS, and JSON is used to represent the body of requests and responses. Requests should be made with an HTTP headers of "Accept: application/json" and "Content-Type: application/json" to indicate that the body of the request will be JSON and that JSON is expected in return. Compression may also be negotiated/used as indicated by HTTP headers (see OpenRTB 3.0 for an example of how this is done).

Breaking changes are restricted to major versions of this specification (1.x, 2.x, etc.).

Almost all fields are optional at a technical level, however exchanges may mandate the presence of certain fields as a business requirement.  Both exchanges and bidders must gracefully deal with the presence of unexpected or unknown fields. Exchanges are recommended to store fields regardless of their relevance to the exchange, as they may hold significance to a bidder. For example, on exchanges which use their own ad IDs, bidders may wish to store their ID in the extension object. 

It is expected that a given exchange operates using either it's own ad IDs or bidder IDs, but not both, for the prevention of ambiguity.

All requests must result in the manipulated object(s) being returned in response. 

## Status Codes <a name="statuscodes"></a>

HTTP status codes are used by the exchange to express the status of requests made by the bidder:

<table>
  <tr>
    <th>Code</th>
    <th>Name</th>
    <th>Description</th>
  </tr>
  <tr>
    <td>200</td>
    <td>OK</td>
    <td>The request was successful.</td>
  </tr>
  <tr>
    <td>400</td>
    <td>Bad Request</td>
    <td>The request could not be interpreted successfully.</td>
  </tr>
  <tr>
    <td>401</td>
    <td>Unauthorized</td>
    <td>The request did not contain correct authentication information.</td>
  </tr>
  <tr>
    <td>404</td>
    <td>Not Found</td>
    <td>The resource does not exist.</td>
  </tr>
  <tr>
    <td>429</td>
    <td>Too Many Requests</td>
    <td>The bidder has exceeded the rate limit set by the exchange and must wait before trying again.</td>
  </tr>
  <tr>
    <td>500</td>
    <td>Internal Service Error</td>
    <td>The exchange has encountered technical difficulties.</td>
  </tr>
</table>


## Sparse Fieldsets <a name="sparsefieldsets"></a>

AdCOM objects returned in responses may contain a sparse fieldset to save bandwidth, where this specification indicates this is acceptable (see "Endpoints" below). In this case fields should be assumed by bidders to be unchanged if not present. Bidders can determine changes made by an exchange by comparing to the object it previously sent. Sparse fieldsets must include at a minimum the following fields, if the object in question is sent:

* Ad object
    * id
    * init (initial ad creation only)
    * lastmod
* Audit object
    * status
    * lastmod

## Dates and Times <a name="datesandtimes"></a>

All dates/times must be specified in the format of millis since Unix epoch (January 1, 1970 00:00:00 UTC). If an implementer internally uses a more precise timestamp, values should always be rounded **down** to the nearest millisecond to so that when queries are made using a filter, ads are not missed due to rounding.

# Endpoints <a name="endpoints"></a>

Bidders will interact with the Ad Management API by making HTTP calls to specific endpoints. Exchanges will specify a **base URL** (denoted using the {baseUrl} placeholder in this document) and a **bidder ID** representing a given bidder/demand partner (denoted using the {bidderId} placeholder in this document). All endpoints are relative to this base URL. The base URL must include the major version of the specification implemented in the form of "v#". For example, an exchange implementing version 1.1 of this API may define its base URL as `https://api.exchange.com/management/v1`. For example, assuming a bidder ID of 492 the ads endpoint will be reached at `https://api.exchange.com/management/v1/bidder/492/ads`.

Per "API Conventions" above, breaking changes require the major version number to be iterated, and partners must deal gracefully with unexpected fields, so the minor version number is not required in the URL and its omission provide ease of upgrade. 

<table>
  <tr>
    <th>Endpoint</th>
    <th>Methods supported</th>
    <th>Description</th>
  </tr>
  <tr>
    <td>{baseUrl}/bidder/{bidderId}/ads</td>
    <td>GET, POST</td>
    <td><strong>GET:</strong> returns a collection of ads for a given bidder. This response may be sparse at the exchange's discretion (see "API conventions"). <br />

An "auditStart" filter, at a minimum, must be set on the query string to constrain the number of ads returned, else exchanges may choose to return a 400 status code. Exchanges may limit the number of ads in the returned collection at their discretion. If the result set is a subset of available ads, this will be indicated in the result (see "Collection of ads"). Bidders may fetch the remaining ads by calling the URL included in the "nextPage" field, repeating until the bidder has gathered all updates. <br />

The available filters are: <br />

**auditStart:** Timestamp, based on "lastmod" value from the Audit object of returned ads, used as the starting point for pagination. See <a href="#pagination">Appendix C: API Pagination</a> for more details. (Required). <br />
**paginationId:** Needed for correct pagination when fetching subsequent pages; an ad ID for returned ads used as the starting point for pagination. See <a href="#pagination">Appendix C: API Pagination</a> for more details. (Optional)<br />
**auditEnd:** Ending timestamp for the "lastmod" value from the Audit object of returned ads (timestamp less than or equal to this value). (Optional, now is assumed if omitted) <br />

See <a href="#apiconventions">API conventions</a> regarding date format and <a href="#pagination">Appendix D: Pagination</a> for details about correctly implementing pagination and using the "auditStart" and "id" fields. <br />

Example:  <br />

`{baseUrl}/bidder/{bidderId}/ads?auditStart=1528221114000&paginationId=421`


<strong>POST:</strong> submits a single ad. The body must contain a only an Ad object (and its children). Returns a collection of ads containing the ad submitted, including any fields or child objects provided by the exchange. This response may be sparse at the exchange's discretion (see "API conventions").</td>
  </tr>
  <tr>
    <td>{baseUrl}/bidder/{bidderId}/ads/{id}</td>
    <td>GET, PUT, PATCH</td>
    <td><strong>GET:</strong> Returns a collection of ads resource containing a single ad in its entirety. <br />
<strong>PUT:</strong> replaces the ad object in its' entirety, and returns a collection of ads containing the specified ad, including any fields or child objects provided by the exchange. This response may be sparse at the exchange's discretion (see "API conventions"). <br />
<strong>PATCH:</strong> replaces only the specified fields, and returns a collection of ads containing the specified ad, including any fields or child objects provided by the exchange. This response may be sparse at the exchange's discretion (see "API conventions").</td>
  </tr>
</table>


# Authentication <a name="authentication"></a>

In this version of the Ad Management API, authentication protocol is left to the discretion of the exchange or SSP implementing the API, and should be discussed with API users a priori.  

# Resource Representations <a name="resourcerepresentations"></a>

Resources are represented using JSON.


## Collection Of Ads <a name="collectionofads"></a>

A collection of ads is an object containing one or more ads with additional metadata.

<table>
  <tr>
    <th>Attribute</th>
    <th>Type</th>
    <th>Description</th>
  </tr>
  <tr>
    <td>count</td>
    <td>integer</td>
    <td>The number of ads in this collection. </td>
  </tr>
  <tr>
    <td>more</td>
    <td>integer</td>
    <td>A boolean flag indicating that this collection is a subset; the number of ads returned has been limited by exchange policy. See "Endpoints" above. May be omitted when not needed.</td>
  </tr>
  <tr>
    <td>nextPage</td>
    <td>string</td>
    <td>A URL for the next page of results when "more" is true, null (or not present) otherwise.  See <a href="#pagination">API Pagination</a> above for details. E.g. `https://{baseUrl}/bidder/{bidderId}/ads?auditStart=1528221114000&paginationId=421`</td>
  </tr>
  <tr>
    <td>ads</td>
    <td>object array</td>
    <td>An array of ad resources. On GET, sorted by oldest to newest date of "lastmod" from the Audit object.</td>
  </tr>
</table>


## Ad <a name="ad"></a>

An ad resource is an object representing each unique ad that will be used by the bidder. It is a AdCOM 1.x ad object with relevant child objects. See the [AdCOM specification](https://github.com/InteractiveAdvertisingBureau/AdCOM) for details.

Only the bidder may modify the ad object and its' children, excepting the Audit object and these fields from the Ad object:

* "id": Set by the exchange (only for exchanges who express ads in terms of their IDs)

* "init": Set by the exchange on initial submission of the ad

* "lastmod": Set by the exchange on the most recently modification to the ad

## Audit <a name="audit"></a>

Subordinate to the Ad object is a AdCOM 1.x audit object. See the [AdCOM specification](https://github.com/InteractiveAdvertisingBureau/AdCOM) for details. Among the many objects that may be present subordinate to the Ad object, it is specifically noted in this specification because its behaviour in the context of ad management is worth elaborating on.

Only the exchange may modify the audit object. Upon initial ad upload, the exchange must initialize the "init" and "lastmod" field to the timestamp of the submission of the ad. The "lastmod" field must be updated at any change of this object.

# Webhooks <a name="webhooks"></a>

Exchanges may choose to support sending webhook calls to bidders to notify them of changes to ad audit status, and bidders may choose to receive such calls. 

It is recommended to use webhooks as a means of reducing the required polling frequency substantially. Bidders should still poll occasionally to ensure that all changes are collected (to account for failures during webhook calls, etc.), but at a much lower frequency (such as once per day).

## Registration and Authentication <a name="webhookauthentication"></a>

In this version of the Ad Management API, the method of registration of a webhook URL and authentication protocol is left to the discretion of the exchange or SSP implementing the API. These matters should be discussed with API users a priori.  

## Webhook Calls <a name="webhookcalls"></a> 

If an exchange supports webhooks and the exchange and the bidder have arranged to enable webhook calls, the exchange will POST a collection of ads to the bidder's webhook URL upon status change for an ad or ad(s). The JSON in the POST may be sparse, containing only changes. The bidder must respond 204 No Content if the webhook is successfully processed. In the event of failures such as timeouts, non-2xx status codes, etc., it is recommended the exchange make at least 3 further attempts with a delay of 30 seconds between each request before abandoning the attempt. It is recommended that exchanges record permanent failures for offline analysis.

# Expiration <a name="expiration"></a>

Exchanges may not wish to maintain a record of Ads indefinitely. Accordingly, the idea of "expiration" is supported. Expiration is an audit status that signals that the exchange is due to delete the Ad in future if no action is taken.

The following are to be decided by the exchange, but must be communicated a priori to bidders:

- The criteria for when an Ad's audit status becomes "expired"
- The criteria for when an Ad is deleted
- Whether Ads with an audit status of "expired" may bid or not

Typically, these criteria will be in the form of days, e.g. an exchange may define a policy like:

- Ad expiration: 30 days of inactivity
- Ad deletion: 120 days after expiry
- Expired ad bidding policy: Bidders must explicitly re-activate an ad (or, alternatively, bidding triggers re-activation)

## Re-activating Expired Ads <a name="reactivate"></a>

If exchanges require explicit re-activation of expired ads, bidders may do so by touching the ad; a PUT or a PATCH against the ad constitutes a touch. 

Whether such re-activated ads can be immediately used or must go through another audit is a matter of exchange policy, and bidders should consult the audit status in the Ad object in the response to the PUT/PATCH request.

# Requesting Re-Audit <a name="reaudit"></a>

Bidders may request that exchanges re-audit ads with an audit status of 4 (Denied) or 5 (Changed; Resubmission Requested) by touching the ad; a PUT or a PATCH against the ad constitutes a touch. Bidders should not request re-audit of ads without addressing the audit feedback returned by exchanges. 

Whether exchanges permit re-audit requests for a given ad (or any ads) is a matter of exchange policy; should an exchange refuse such a request, the audit status in the Ad object in the response to the PUT/PATCH request should be left unchanged.

# Substitution Macros <a name="substitutionmacros"></a>

In the "exchange-hosted ad markup", since bidders do not include their ad markup in the bid response, the exchange must provide the facility for custom macros so that the bidder can provide any variable details (such as cachebusters, impression ID, product IDs, etc.) needed to serve the ad. These custom macros are substituted by the exchange when serving. The strings which may be used are addressed by the transaction specification used for bidding, for example OpenRTB 3.0. 

The values for these custom macros are supplied by the bidder during bidding.

# Typical Synchronization Flow <a name="typicalsynchronizationflow"></a>

Authentication is not shown here for brevity.

As ads are created, the bidder places the ads into a queue for submission. The bidder makes one or more HTTP POST calls to:

`{baseUrl}/bidder/{bidderId}/ads`

... with a body containing a single Ad resource. Exchanges will periodically update the audit object inside the ad with modifications to the **status** (e.g. to set ads as approved or denied), and any other fields as a result of the exchange's assessment of the ad. Bidders should expect that that fields in the audit object could change at any time.

As ads change audit status, the exchange makes HTTP POST calls to pre-arranged bidder webhook URL, with each call containing a collection of ads (one or more).

Bidders may also choose to periodically poll for updates (thereby preventing missed information if webhook calls fail) by making HTTP GET calls to:

`{baseUrl}/bidder/{bidderId}/ads?auditStart={timestamp}`

Bidders should record the audit "lastmod" for each ad, and choose an auditStart value based on the most recently observed audit update timestamp.

Alternatively, bidders may query for the status of a particular ad by making an HTTP GET call to:

`{baseUrl}/bidder/{bidderId}/ads/{id}`

Bidders should make use of the information they receive to change their bidding behaviour appropriately.

If the bidder has made local changes to an ad, the bidder makes an HTTP PATCH or PUT call to:

`{baseUrl}/bidder/{bidderId}/ads/{id}`

... to update the ad record at the exchange. On an exchange with restrictive bidding, typically, this would result in the ad returning to status 1, "pending audit", but this is a matter of exchange business rules. For example, an exchange might deem only a subset of changes to be significant enough to require a re-audit (such as a change in landing page domain). It is up to exchanges to communicate what deviations constitute a material change to the ad (and thus may trigger a change in audit status).

# Appendix A: Integration Checklist <a name="appendixa_integrationchecklist"></a>

To facilitate integration, exchanges should provide a document similar to the below to inform bidders about the specifics of an exchange's implementation of this spec.

<table>
  <tr>
    <th>Exchange</th>
    <td></td>
  </tr>
  <tr>
    <th>Base URL</th>
    <td></td>
  </tr>
  <tr>
    <th>Bidder ID</th>
    <td></td>
  </tr>
  <tr>
    <th>Version Used</th>
    <td></td>
  </tr>
  <tr>
    <th>Rate Limit</th>
    <td></td>
  </tr>
  <tr>
    <th>Max Ads per Response</th>
    <td></td>
  </tr>
  <tr>
    <th>Markup and Bidding Policy</th>
    <td></td>
  </tr>
  <tr>
  	<th>Ad Retention Policy</th>
    <td>(Days to expiry, days to deletion, bidding behaviour for expired ads</td>
  </tr>
  <tr>
    <th>Ad IDs of Record</th>
    <td>(Bidder or Exchange)</td>
  </tr>
  <tr>
    <th>Attributes Required</th>
    <td></td>
  </tr>
  <tr>
    <th>Notes</th>
    <td>Additional notes of relevance regarding this implementation.</td>
  </tr>
</table>

# Appendix B: Examples <a name="appendixb_examples"></a>

## Minimal Implementation <a name="minimalimplementation"></a>

<table>
  <tr>
    <th>Exchange</th>
    <td>SuperAds</td>
  </tr>
  <tr>
    <th>Base URL</th>
    <td>https://api.superads.com/management/v1</td>
  </tr>
  <tr>
    <th>Bidder ID</th>
    <td>496</td>
  </tr>
  <tr>
    <th>Version Used</th>
    <td>v1.1</td>
  </tr>
  <tr>
    <th>Rate Limit</th>
    <td>100 requests per minute</td>
  </tr>
  <tr>
    <th>Max Ads per Response</th>
    <td>100</td>
  </tr>
  <tr>
    <th>Markup and Bidding Policy</th>
    <td>Permissive bidding, ad markup in bid response</td>
  </tr>
  <tr>
  	<th>Ad Retention Policy</th>
    <td>Ads expire after 14 days of inactivity and are deleted after 30 days in expired status. Expired ads may be used in bidding and will be automatically re-activated.</td>
  </tr>
  <tr>
    <th>Ad IDs of Record</th>
    <td>Bidder</td>
  </tr>
  <tr>
    <th>Attributes Required</th>
    <td>Required in Ad object: adomain, iurl, one of display or video
Required in Display object: w, h</td>
  </tr>
</table>


This exchange is not interested in receiving the markup itself, merely a representative inspection image ('iurl') to be reviewed by auditors. This is representative of the situation on some mobile exchanges at time of publication.

**NOTE:** Such an implementation would not provide the option for the exchange to scan markup for malicious content. While the API supports such an implementation, a more sophisticated implementation is recommended.

### Bidder Ad Submission <a name="bidderadsubmission1"></a>

POST `https://api.superads.com/management/v1/bidder/496/ads`

```json
{
  "id": "557391",
  "adomain": "advertiser.com",
  "iurl": "http://cdn.dsp.com/iurls/557391.gif",
  "display": {
    "w": 300,
    "h": 250
  }
}
```

Response:

```json
{
  "count": 1,
  "ads": [
    {
      "id": "557391",
      "init": 1528221112000,
      "lastmod": 1528221112000,
      "audit": {
        "status": 2,
        "lastmod": 1528221112000
      }
    }
  ]
}
```

### Bidder Receives A Webhook Call From Exchange <a name="bidderreceivesawebhookcallfromexchange1"></a>

In this case, notifying it that an ad has been disapproved.

POST `{hookurl}`

```json
{
  "count": 1,
  "ads": [
    {
      "id": "557391",
      "lastmod": 1528221112000,
      "audit": {
        "status": 4,
        "feedback": "Content disallowed by exchange policy.",
        "lastmod": 1528288587000
      }
    }
  ]
}
```

### Bidder Polls For Updates <a name="bidderpollsforupdates1"></a>

The bidder is requesting all ads whose status has changed since the most recent audit status change observed on last poll (for this example, timestamp 1528282813000). In this example, there are more ads that have changed than the maximum the exchange will return in a single call. The bidder follows the URL provided in "nextPage" to get the next page of results.

GET `https://api.superads.com/management/v1/bidder/496/ads?auditStart=1528282813000`

```json
{
  "count": 100,
  "more": 1,
  "nextPage": "https://api.superads.com/management/v1/bidder/496/ads?auditStart=1528306991000&paginationId=557398",
  "ads": [
    {
      "id": "557391",
      "lastmod": 1528221112000,
      "audit": {
        "status": 4,
        "feedback": "Content disallowed by exchange policy.",
        "lastmod": 1528288587000
      }
    },
    {
      "id": "557533",
      "lastmod": 1528289509000,
      "audit": {
        "status": 3,
        "lastmod": 1528289509000
      }
    },
    { ... },
    {
      "id": "557398",
      "lastmod": 1528222962000,
      "audit": {
        "status": 3,
        "lastmod": 1528306991000
      }
    }
  ]
}
```

GET `https://api.superads.com/management/v1/bidder/496/ads?auditStart=1528306991000&paginationId=557398`

```json
{
  "count": 2,
  "more": 0,
  "ads": [
    {
      "id": "557231",
      "lastmod": 1528227434000,
      "audit": {
        "status": 4,
        "feedback": "Content disallowed by exchange policy.",
        "lastmod": 1528307136000
      }
    },
    {
      "id": "557599",
      "lastmod": 1528042889000,
      "audit": {
        "status": 3,
        "lastmod": 1528307223000
      }
    }
  ]
}
```

As this is the last page of results, "nextPage" is not included by the exchange and "more" is 0.

## Typical Implementation <a name="typicalimplementation"></a>

<table>
  <tr>
    <th>Exchange</th>
    <td>AdvancedAds</td>
  </tr>
  <tr>
    <th>Base URL</th>
    <td>https://api.advancedads.com/admgmt/v1</td>
  </tr>
  <tr>
    <th>Bidder ID</th>
    <td>34</td>
  </tr>
  <tr>
    <th>Version Used</th>
    <td>v1.1</td>
  </tr>
  <tr>
    <th>Rate Limit</th>
    <td>100 requests per minute</td>
  </tr>
  <tr>
    <th>Max Ads per Response</th>
    <td>100</td>
  </tr>
  <tr>
    <th>Markup and Bidding Policy</th>
    <td>Restrictive bidding, ad markup in bid response</td>
  </tr>
  <tr>
  	<th>Ad Retention Policy</th>
    <td>Ads expire after 14 days of inactivity and are deleted after 30 days in expired status. Bidders must explicitly re-activate expired ads.</td>
  </tr>
  <tr>
    <th>Ad IDs of Record</th>
    <td>Bidder</td>
  </tr>
  <tr>
    <th>Attributes Required</th>
    <td>Required in Ad: adomain, cat, one of display or video <br />
Required in Display: one of adm or banner, one of w + h or wratio + hratio, type <br />
Required in Banner: img, link <br />
Required in Video: one of adm or curl, api, mime, type</td>
  </tr>
</table>


### Bidder Ad Submission <a name="bidderadsubmission2"></a>

POST `https://api.advancedads.com/admgmt/v1/bidder/34/ads`

```json
{
  "id": "557391",
  "cat": "653",
  "adomain": "ford.com",
  "display": {
    "w": 300,
    "h": 250,
    "secure": 1,
    "adm": "<!-- Markup -->"
  }
}
```

Response:

```json
{
  "count": 1,
  "ads": [
    {
      "id": "557391",
      "init": 1528221112000,
      "lastmod": 1528221112000,
      "audit": {
        "status": 1,
        "lastmod": 1528221112000
      }
    }
  ]
}
```

Given the bidding policy of the exchange and the initial audit status returned, the bidder cannot use this ad in bidding until it receives an update to the audit status.

### Bidder Polls For Updates <a name="bidderpollsforupdates2"></a>

GET `https://api.advancedads.com/admgmt/v1/bidder/34/ads?auditStart=1528282813000`

```json
{
  "count": 4,
  "more": 0,
  "ads": [
    { ... },
    {
      "id": "557391",
      "lastmod": 1528221112000,
      "audit": {
        "status": 3,
        "feedback": "Corrected category. Added missing attribute.",
        "corr": {
          "cat": "1",
          "attr": 6
        },
        "lastmod": 1528288587000
      }
    }
  ]
}
```

In the above scenario, the exchange has approved the ad but has updated it to reflect that, in the its opinion, the ad should be classified as category Automotive, and has auto-play in-banner video. The equivalent webhook call would be similar (see above example).

### Bidder Re-activates an Ad <a name="bidderreactivates"></a>

The bidder may find an ad to have an audit status of 6 (Expired) either upon polling for changes or through a webhook call. Given the policy of this exchange, such an ad is not eligible for bidding. If the bidder wishes to use the ad again, it can simply touch the ad to trigger its re-activation.

PATCH `https://api.advancedads.com/admgmt/v1/bidder/34/ads/557391`

```json
{}
```

Response:

```json
{
  "count": 1,
  "ads": [
    {
      "id": "557391",
      "init": 1528221112000,
      "lastmod": 1529052323000,
      "audit": {
        "status": 1,
        "lastmod": 1529052323000
      }
    }
  ]
}
```

# Appendix C: API Pagination <a name="pagination"></a>

When it comes to implementing pagination, there are subtleties that implementers must consider to ensure correct implementation, otherwise an infinite loop or missing ads could occur.

In order to address this, Ad Management API uses a timestamp + ID pattern similar to that [described here](https://phauer.com/2018/web-api-pagination-timestamp-id-continuation-token/). The beginning timestamp for ads returned in the collection of ads is given by the "auditStart" query parameter. This timestamp is based on "lastmod" from the Audit object of the ads. The "paginationId" query parameter is the beginning ID for ads returned, and is used for fetching subsequent pages.

The following needs to be covered to ensure correct implementation:
* The collection of ads returned for each page does not contain records repeated from the prior page (unless the timestamp for last modification has changed since the last request)
* No ads are skipped even if there is an identical timestamp for multiple ads.
* Ads returned in a [collection of ads](#collectionofads) must be sorted by the "lastmod" timestamp ascending and the "paginationId" ascending, in that order.
* * While examples here use numeric values for "paginationId", this is not required. For example, in a given integration between a DSP and exchange, the DSP's ad IDs might be used, and these might be alphanumeric strings.  All that is essential is that sort is consistent.   
* When **only** "auditStart" is specified for the request, any ads with a "lastmod" **greater than** the specified timestamp will be returned (up to the maximum of number of ads an exchange is willing to return per page).
* When "auditStart" and "paginationId" are **both** specified for the request, any ads with a) "lastmod" **equal to** the specified timestamp and "paginationId" **greater than** the specified ID or b) "lastmod" **greater than** the specified timestamp will be returned (up to the exchange's maximum per page). 


# Appendix D: Resources <a name="appendixd_resources"></a>

Interactive Advertising Bureau Technology Laboratory (IAB Tech Lab)  
[www.iabtechlab.com](https://www.iabtechlab.com)

OpenMedia Specification Stack    
[https://iabtechlab.com/openmedia](https://iabtechlab.com/openmedia)

AdCOM Project on Github  
[https://github.com/InteractiveAdvertisingBureau/AdCOM](https://github.com/InteractiveAdvertisingBureau/AdCOM)

OpenRTB v3.0 Specification  
[https://github.com/InteractiveAdvertisingBureau/openrtb](https://github.com/InteractiveAdvertisingBureau/openrtb)

# Appendix E:  Change Log <a name="appendixe_changelog"></a>

This appendix serves as a brief summary of changes to the specification. These changes pertain only to the substance of the specification and not routine document formatting, information organization, or content without technical impact. For that, see [Appendix F: Errata](#appendixf_errata).

<table>
  <tr>
    <td><strong>Version</strong></td>
    <td><strong>Changes</strong></td>
  </tr>
  <tr>
    <td>1.1</td>
    <td><b>Enhanced pagination:</b> As specified in v1.0, an infinite loop could occur while attempting to paginate through results. This could occur if there were more ads with an identical last modification timestamp than an exchange will return per page. To address this, non-breaking enhancements have been added for a more sophisticated approach to pagination.<br />
  </tr>
  <tr>
    <td>1.0</td>
    <td><b>Initial release.</b><br />
  </tr>
</table>

# Appendix F: Errata <a name="appendixf_errata"></a>

This appendix catalogues any error corrections which have been made to this document after its versioned release. The body of the document has been updated accordingly.

Only minor fixes, such as clarifications or corrections to descriptions, may be treated as errata. Improvements or material changes are summarized in the change log.

Granular details of the changes can be seen by reviewing the commit history of the document.

There are no errata at this time.