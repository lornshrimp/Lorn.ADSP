![IAB Tech Lab](https://drive.google.com/uc?id=10yoBoG5uRETSXRrnJPUDuONujvADrSG1)

# **AdCOM 1.0**

#### Executive Summary
One of the most successful IAB standards is OpenRTB.  This protocol for conducting real-time auctions among sell-side exchanges and demand-side bidders first launched as OpenRTB v1.0 Mobile in February 2011.  Later that year, OpenRTB v2.0 was released, which provided a unified protocol for mobile, display, and video.  Due to widespread industry adoption, OpenRTB was established as an IAB standard in January 2012 with the release of version v2.1 although governance over technical content remained with the OpenRTB community.  Since then and true to its initial objective, OpenRTB has become the lingua franca of real-time programmatic advertising and entered 2018 as v2.5.

During these years, programmatic advertising has become a dominant force in the industry.  However, this has also led to an increasingly complex supply chain which may increase fraud rates and other risks.  This is one of the key motivators driving OpenRTB v3.0 since the level of change needed to meet the challenges of programmatic currently and going forward could not be accomplished in a backward compatible manner (i.e., as an additional v2.x release).

Combined with the OpenMedia goal of rationalizing the IAB standards portfolio, this has led to a layered approach, where OpenRTB will focus on the actual media commerce transaction (e.g., auction parameters, deals, bids, etc.) while the concepts in common with other specifications (e.g., ads, placements, users, devices, sites, publishers, etc.) will be factored into its own reusable specification.  Thus the genesis of the Advertising Common Object Model or AdCOM.

In addition to providing the modularity to benefit specifications in addition to OpenRTB, AdCOM seeks to address other business challenges of programmatic advertising.  For example, publishers currently have limited ability to control the types of creatives they run on their properties due to the opaque nature in which traditional display ads are conveyed.  Many types of undesirable creatives make their way to publisher content such as overly heavy payloads, those lacking of brand safety, excessive pixel fires, and JavaScript that launches malware.

These and other problematic behaviors result in poor and potentially damaging user experiences, diminished user trust, installation of ad blockers, erosion of publisher monetization, and the increased challenge for good actors on the advertiser side to reach their intended audience.  AdCOM attempts to address these challenges by supporting new and safer structured ad formats.

Reusability by multiple IAB specifications positions AdCOM to leverage solutions such as this across a range of industry applications.

#### About This Repository
At all times, the **master** branch of this repository contains the most recent release of AdCOM.  See ["AdCOM v1.0 Final.md"](https://github.com/InteractiveAdvertisingBureau/AdCOM/blob/master/AdCOM%20v1.0%20FINAL.md) in the master branch for the latest specification.

Branches exist for prior releases. Use these to review detailed changes from one release to another. A brief change log is found in the spec itself.

The **develop** branch contains work-in-progress for an upcoming release. It may change at any time.

#### OpenMedia
https://iabtechlab.com/openmedia 

#### OpenRTB 
https://github.com/InteractiveAdvertisingBureau/openrtb

#### Contact
For more information, or to get involved, please email openmedia@iabtechlab.com.

#### About IAB Tech Lab  

The IAB Technology Laboratory (Tech Lab) is a non-profit research and development consortium that produces and provides standards, software, and services to drive growth of an effective and sustainable global digital media ecosystem. Comprised of digital publishers and ad technology firms, as well as marketers, agencies, and other companies with interests in the interactive marketing arena, IAB Tech Lab aims to enable brand and media growth via a transparent, safe, effective supply chain, simpler and more consistent measurement, and better advertising experiences for consumers, with a focus on mobile and TV/digital video channel enablement. The IAB Tech Lab portfolio includes the DigiTrust real-time standardized identity service designed to improve the digital experience for consumers, publishers, advertisers, and third-party platforms. Board members include AppNexus, ExtremeReach, Google, GroupM, Hearst Digital Media, Integral Ad Science, Index Exchange, LinkedIn, MediaMath, Microsoft, Moat, Pandora, PubMatic, Quantcast, Telaria, The Trade Desk, and Yahoo! Japan. Established in 2014, the IAB Tech Lab is headquartered in New York City with an office in San Francisco and representation in Seattle and London.

Learn more about IAB Tech Lab here: [https://www.iabtechlab.com/](https://www.iabtechlab.com/)


#### Contributors and Technical Governance

OpenRTB Working Group members provide contributions to this repository. Participants in the OpenRTB Working group must be members of IAB Tech Lab. Technical Governance and code commits for the project are provided by the IAB Tech Lab OpenRTB Commit Group. 

### License
OpenRTB Specification the IAB Tech Lab is licensed under a Creative Commons Attribution 3.0 License.   To view a copy of this license, visit creativecommons.org/licenses/by/3.0/ or write to Creative Commons, 171 Second Street, Suite 300, San Francisco, CA 94105, USA.

By submitting an idea, specification, software code, document, file, or other material (each, a “Submission”) to the AdCOM repository, to any member of the Programmatic Supply Chain Working Group, or to the IAB Tech Lab in relation to AdCOM you agree to and hereby license such Submission to the IAB Tech Lab under the Creative Commons Attribution 3.0 License and agree that such Submission may be used and made available to the public under the terms of such license. If you are a member of the IAB Tech Lab then the terms and conditions of the [IPR Policy](https://iabtechlab.com/ipr-iab-techlab/acknowledge-ipr/) may also be applicable to your Submission, and if the IPR Policy is applicable to your Submission then the IPR Policy will control  in the event of a conflict between the Creative Commons Attribution 3.0 License and the IPR Policy.

#### Disclaimer

THE STANDARDS, THE SPECIFICATIONS, THE MEASUREMENT GUIDELINES, AND ANY OTHER MATERIALS OR SERVICES PROVIDED TO OR USED BY YOU HEREUNDER (THE “PRODUCTS AND SERVICES”) ARE PROVIDED “AS IS” AND “AS AVAILABLE,” AND IAB TECHNOLOGY LABORATORY, INC. (“TECH LAB”) MAKES NO WARRANTY WITH RESPECT TO THE SAME AND HEREBY DISCLAIMS ANY AND ALL EXPRESS, IMPLIED, OR STATUTORY WARRANTIES, INCLUDING, WITHOUT LIMITATION, ANY WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE, AVAILABILITY, ERROR-FREE OR UNINTERRUPTED OPERATION, AND ANY WARRANTIES ARISING FROM A COURSE OF DEALING, COURSE OF PERFORMANCE, OR USAGE OF TRADE. TO THE EXTENT THAT TECH LAB MAY NOT AS A MATTER OF APPLICABLE LAW DISCLAIM ANY IMPLIED WARRANTY, THE SCOPE AND DURATION OF SUCH WARRANTY WILL BE THE MINIMUM PERMITTED UNDER SUCH LAW. THE PRODUCTS AND SERVICES DO NOT CONSTITUTE BUSINESS OR LEGAL ADVICE. TECH LAB DOES NOT WARRANT THAT THE PRODUCTS AND SERVICES PROVIDED TO OR USED BY YOU HEREUNDER SHALL CAUSE YOU AND/OR YOUR PRODUCTS OR SERVICES TO BE IN COMPLIANCE WITH ANY APPLICABLE LAWS, REGULATIONS, OR SELF-REGULATORY FRAMEWORKS, AND YOU ARE SOLELY RESPONSIBLE FOR COMPLIANCE WITH THE SAME, INCLUDING, BUT NOT LIMITED TO, DATA PROTECTION LAWS, SUCH AS THE PERSONAL INFORMATION PROTECTION AND ELECTRONIC DOCUMENTS ACT (CANADA), THE DATA PROTECTION DIRECTIVE (EU), THE E-PRIVACY DIRECTIVE (EU), THE GENERAL DATA PROTECTION REGULATION (EU), AND THE E-PRIVACY REGULATION (EU) AS AND WHEN THEY BECOME EFFECTIVE.

