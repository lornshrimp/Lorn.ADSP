![IAB Tech Lab](https://drive.google.com/uc?id=10yoBoG5uRETSXRrnJPUDuONujvADrSG1)

# OpenRTB Advisory for Open Measurement SDK 

**Working Draft** | July 2018

### Scope

This document advises OpenRTB buy-side implementers (DSPs, buyers) how to read signals from sellers indicating the presence of IAB Tech Lab's new viewability tool, the Open Measurement SDK. This document is complementary to the guidance given in [Open Measurement SDK Onboarding Guide for Integration Partners](https://s3-us-west-2.amazonaws.com/omsdk-files/docs/IAB+Open+Measurement+SDK+Onboarding_GA+version.pdf).

OpenRTB 3.0 and AdCOM 1.0 are in beta from July 24 to September 24, 2018, allowing for public comments and beta testing. The guidance for implementation of OMSDK is released alongside the beta release of OpenRTB 3.0 and AdCOM 1.0.

### About OpenRTB

The RTB Project, formerly known as the OpenRTB Consortium, assembled in November 2010 to develop an API specification for companies interested in an open protocol for the automated trading of digital media across a broader range of platforms, devices, and advertising solutions. 

OpenRTB 2.5 is the most recent version for public adoption and OpenRTB 3.0 has been through public review, to be finalized later in 2018. 

OpenRTB versions 2.1-2.5 can be found at [https://iabtechlab.com/openrtb](https://iabtechlab.com/openrtb) 

For more information about the OpenRTB specifications and the OpenRTB working group: [https://iabtechlab.com/openrtb](https://iabtechlab.com/openrtb) 

### About Open Measurement SDK

The Open Measurement Software Development Kit (OM SDK) is designed to facilitate third party viewability and verification measurement for ads served to mobile app environments without requiring multiple Ad Verification Service Providers (Measurement Provider) Software Development Kits (SDKs). OM SDK is developed and managed by the Open Measurement Working Group (OMWG).

For more information about the OMSDK  and the OMWG: [https://iabtechlab.com/omsdk](https://iabtechlab.com/omsdk) 

### About IAB Tech Lab

The IAB Technology Laboratory (Tech Lab) is a non-profit research and development consortium that produces and provides standards, software, and services to drive growth of an effective and sustainable global digital media ecosystem. Comprised of digital publishers and ad technology firms, as well as marketers, agencies, and other companies with interests in the interactive marketing arena, IAB Tech Lab aims to enable brand and media growth via a transparent, safe, effective supply chain, simpler and more consistent measurement, and better advertising experiences for consumers, with a focus on mobile and TV/digital video channel enablement. The IAB Tech Lab portfolio includes the DigiTrust real-time standardized identity service designed to improve the digital experience for consumers, publishers, advertisers, and third-party platforms. Board members include AppNexus, ExtremeReach, Google, GroupM, Hearst Digital Media, Integral Ad Science, Index Exchange, LinkedIn, MediaMath, Microsoft, Moat, Pandora, PubMatic, Quantcast, Telaria, The Trade Desk, and Yahoo! Japan. Established in 2014, the IAB Tech Lab is headquartered in New York City with an office in San Francisco and representation in Seattle and London.

Learn more about IAB Tech Lab here: https://www.iabtechlab.com/

### License

OpenRTB Specification the IAB Tech Lab is licensed under a Creative Commons Attribution 3.0 License.   To view a copy of this license, visit[ creativecommons.org/licenses/by/3.0/](http://creativecommons.org/licenses/by/3.0/) or write to Creative Commons, 171 Second Street, Suite 300, San Francisco, CA 94105, USA.



# OM SDK support in OpenRTB Version 2.0 to 2.5

OpenRTB 2.0 through 2.5 can be easily extended to support OMSDK by communicating: 

1. If OMID is available for a given impression in a bid request 

2. If the ad being used in the bid response requires OMID 

3. The "partner name" assigned to Publishers who have installed the OMSDK

To support this, OpenRTB's list of API Frameworks should be extended with a value for OMID. Extend List 5.6 API Frameworks as follows: 

## List 5.6 API Frameworks

<table>
  <tr>
    <td>Value</td>
    <td>Description </td>
  </tr>
  <tr>
    <td>1</td>
    <td>VPAID 1.0</td>
  </tr>
  <tr>
    <td>2</td>
    <td>VPAID 2.0</td>
  </tr>
  <tr>
    <td>3</td>
    <td>MRAID-1</td>
  </tr>
  <tr>
    <td>4</td>
    <td>ORMMA</td>
  </tr>
  <tr>
    <td>5</td>
    <td>MRAID-2</td>
  </tr>
  <tr>
    <td>6</td>
    <td>MRAID-3</td>
  </tr>
  <tr>
    <td>7</td>
    <td>OMID-1</td>
  </tr>
</table>


In OpenRTB 2.0 and 2.1, value 6 is not present. If extending an existing OpenRTB 2.0 or 2.1 implementation, is is suggested that value 6 is skipped and 7 is used to indicate OMID support to preserve an upgrade path.

## Bid Request

The "api" field of the **Banner, Video, or Native Object** should be used to indicate availability of OMID. This is supported in OpenRTB 2.0+.

### Banner/Video/Native/Audio Object

<table>
  <tr>
    <td>Attribute</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>api</td>
    <td>integer array</td>
    <td>List of supported API Frameworks for this impression. Refer to List 5.6. If an API is not explicitly listed, it is assumed not to be supported.</td>
  </tr>
</table>


## Bid Response

The "apis" and "api" fields of the **Bid Object** should be used to indicate that the ad being used in the bid requires use of OMID. The "apis" field is supported in OpenRTB 2.6 and "api" field is supported in OpenRTB 2.4+. Though consistent with the design principles of OpenRTB, it should be possible to retrofit these fields into earlier OpenRTB versions as SSPs and DSPs should gracefully ignore fields they do not understand.

### Bid Object

<table>
  <tr>
    <td>Attribute</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>apis</td>
    <td>integer array</td>
    <td>A list of APIs required by the markup if applicable. Refer to List 5.6.</td>
  </tr>
  <tr>
    <td>api</td>
    <td>integer</td>
    <td><i>NOTE: Deprecated in favor of the apis integer array in OpenRTB 2.6.</i> API required by the markup if applicable. Refer to List 5.6.</td>
  </tr>
</table>


# Specific Guidance for Native Ads

OpenRTB Native 1.2 supports OMID with some additional extensions via the 'eventtrackers' object.  This should be used in "Event Trackers Request Object" (section 4.7 of OpenRTB Dynamic [Native Ads API Specification Version 1.2](https://www.iab.com/wp-content/uploads/2018/03/OpenRTB-Native-Ads-Specification-Final-1.2.pdf)).

Suggested values to use for "Event Trackers Request Object" are:

* **_Event_** should be set to a custom value of "555" to signify that this is OMID and the SSP and publisher will determine when to execute OMID script.

* **_Method_** should always be "2" for JS.  Previous guidance to use '555' is not necessary unless the script is non-JS, non-img.

* The javascript **_URL_** should be inserted into the standard eventrackers.url field

* OMID event trackers can include two extension values, which SSP/publisher must pass to OMID when provided:

    * "vendorKey"

    * "verification_parameters"

<table>
  <tr>
    <td>Object</td>
    <td>Value</td>
    <td>Name</td>
  </tr>
  <tr>
    <td>event</td>
    <td>555 </td>
    <td>OMID - new custom value</td>
  </tr>
  <tr>
    <td>methods</td>
    <td>2 </td>
    <td>JS (existing value of 2)</td>
  </tr>
</table>


**Sample bid response**

```
"eventtrackers" { 
  "event": 555  // New, custom value to signify OMID  
  "method": 2  // Previous guidance recommended a value of '555' but '2' is preferred for Javascript  
  "url": "https://verification.com/omid_verification.js"  // This is where DSPs add their actual Javascript URL for measurement  
  "ext": {     // Custom extensions that any exchange can support; these extensions are strongly recommended for SSPs and DSPs to support as part of their OMID support	  
    "vendorKey":  // The vendor key for the company providing the OMID javascript, e.g. "company.com-omid"  
    "verification_parameters":  //Additional verification parameters for the OMSDK, populated by e.g,. "[parameters string]"
	}

}
```


# **OpenRTB and AdCOM**

Bid requests will include two new fields to identify the partner integration.  OpenRTB 2.x and 3.0 will carry these fields in different objects, but the field names and types are the same in both protocols.  

<table>
  <tr>
    <td>Attribute</td>
    <td>Type</td>
    <td>Description</td>
  </tr>
  <tr>
    <td>omidpn</td>
    <td>string </td>
    <td>Identifier of the OM SDK integration.  This is the same as the "name" parameter of the OMID Partner object.</td>
  </tr>
  <tr>
    <td>omidpv</td>
    <td>string</td>
    <td>Version of the OM SDK integration.  This is the same as the "versionString" parameter of the OMID Partner object.</td>
  </tr>
</table>


Note that these fields are in addition to the API frameworks array, which is carried in an "api" field in OpenRTB 2.x and an 'apis' field in AdCOM.  The API frameworks array must include the value 7 to indicate OMID 1.0 support.

### **OpenRTB 2.x**

The two fields will be added to an extension object within the bid request.  There are several options for the extension object to use.  This section lists three possibilities.

#### *BidRequest.Source.Ext*

The object BidRequest.Source describes the source of the OpenRTB 2.x bid request.  Adding "omidpn" and 'omidpv' to Source.Ext would be appropriate if certification applies to all placements in the bid request.

```
"source" {  
  "ext": {  
    "omidpn": "MyIntegrationPartner",  
    "omidpv": "7.1"  
   }  
},  
"imp" [{  
  "banner": {  
    "api": [7]  
  }  
}]  
```


### OpenRTB 3.0 and AdCOM 1.0  (Currently in Beta)

The new AdCOM spec represents ad request data in OpenRTB 3.0.  Unlike OpenRTB 2.x, the "omidpn" and 'omidpv' fields will be extensions in the 'events' object defined in the protocol (AdCOM 1.0 is currently in Beta).  

#### *Event Trackers*

The OMID capability of a bid request is encoded in an "events" object, making it a logical place for the partner information.

```
"events" {
  "apis": [7]  
  "ext": {  
 	 "omidpn": "MyIntegrationPartner",  
 	 "omidpv": "7.1"  
}
```

# OM SDK Guidance

More information about the Open Measurement SDK, including [onboarding guidance](https://s3-us-west-2.amazonaws.com/omsdk-files/docs/IAB+Open+Measurement+SDK+Onboarding_GA+version.pdf ), is available at [https://iabtechlab.com/omsdk](https://iabtechlab.com/omsdk) 

## Support 

If you need support to integrate OM SDK in your app or Ad SDK or have any questions about how to use OM SDK, please send an email to omsdksupport@iabtechlab.com. 
