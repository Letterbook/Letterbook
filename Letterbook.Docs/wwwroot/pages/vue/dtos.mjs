/* Options:
Date: 2023-06-16 04:45:37
Version: 6.90
Tip: To override a DTO option, remove "//" prefix before updating
BaseUrl: https://blazor-gallery-api.jamstacks.net

//AddServiceStackTypes: True
//AddDocAnnotations: True
//AddDescriptionAsComments: True
//IncludeTypes: 
//ExcludeTypes: 
//DefaultImports: 
*/

"use strict";
export class SubType {
    /** @param {{id?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    name;
}
export class AllTypes {
    /** @param {{id?:number,nullableId?:number,byte?:number,short?:number,int?:number,long?:number,uShort?:number,uInt?:number,uLong?:number,float?:number,double?:number,decimal?:number,string?:string,dateTime?:string,timeSpan?:string,dateTimeOffset?:string,guid?:string,char?:string,keyValuePair?:KeyValuePair<string, string>,nullableDateTime?:string,nullableTimeSpan?:string,stringList?:string[],stringArray?:string[],stringMap?:{ [index: string]: string; },intStringMap?:{ [index: number]: string; },subType?:SubType,point?:string,aliasedName?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?number} */
    nullableId;
    /** @type {number} */
    byte;
    /** @type {number} */
    short;
    /** @type {number} */
    int;
    /** @type {number} */
    long;
    /** @type {number} */
    uShort;
    /** @type {number} */
    uInt;
    /** @type {number} */
    uLong;
    /** @type {number} */
    float;
    /** @type {number} */
    double;
    /** @type {number} */
    decimal;
    /** @type {string} */
    string;
    /** @type {string} */
    dateTime;
    /** @type {string} */
    timeSpan;
    /** @type {string} */
    dateTimeOffset;
    /** @type {string} */
    guid;
    /** @type {string} */
    char;
    /** @type {KeyValuePair<string, string>} */
    keyValuePair;
    /** @type {?string} */
    nullableDateTime;
    /** @type {?string} */
    nullableTimeSpan;
    /** @type {string[]} */
    stringList;
    /** @type {string[]} */
    stringArray;
    /** @type {{ [index: string]: string; }} */
    stringMap;
    /** @type {{ [index: number]: string; }} */
    intStringMap;
    /** @type {SubType} */
    subType;
    /** @type {string} */
    point;
    /** @type {string} */
    aliasedName;
}
export class Poco {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
}
export class AllCollectionTypes {
    /** @param {{intArray?:number[],intList?:number[],stringArray?:string[],stringList?:string[],pocoArray?:Poco[],pocoList?:Poco[],nullableByteArray?:Nullable[],nullableByteList?:number[],nullableDateTimeArray?:Nullable[],nullableDateTimeList?:string[],pocoLookup?:{ [index: string]: Poco[]; },pocoLookupMap?:{ [index: string]: { [index:string]: Poco; }[]; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number[]} */
    intArray;
    /** @type {number[]} */
    intList;
    /** @type {string[]} */
    stringArray;
    /** @type {string[]} */
    stringList;
    /** @type {Poco[]} */
    pocoArray;
    /** @type {Poco[]} */
    pocoList;
    /** @type {Nullable[]} */
    nullableByteArray;
    /** @type {number[]} */
    nullableByteList;
    /** @type {Nullable[]} */
    nullableDateTimeArray;
    /** @type {string[]} */
    nullableDateTimeList;
    /** @type {{ [index: string]: Poco[]; }} */
    pocoLookup;
    /** @type {{ [index: string]: { [index:string]: Poco; }[]; }} */
    pocoLookupMap;
}
export class QueryBase {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {?number} */
    skip;
    /** @type {?number} */
    take;
    /** @type {string} */
    orderBy;
    /** @type {string} */
    orderByDesc;
    /** @type {string} */
    include;
    /** @type {string} */
    fields;
    /** @type {{ [index: string]: string; }} */
    meta;
}
/** @typedef T {any} */
export class QueryData extends QueryBase {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
}
/** @typedef {'Leader'|'Player'|'NonPlayer'} */
export var PlayerRole;
(function (PlayerRole) {
    PlayerRole["Leader"] = "Leader"
    PlayerRole["Player"] = "Player"
    PlayerRole["NonPlayer"] = "NonPlayer"
})(PlayerRole || (PlayerRole = {}));
/** @typedef {number} */
export var PlayerRegion;
(function (PlayerRegion) {
    PlayerRegion[PlayerRegion["Africa"] = 1] = "Africa"
    PlayerRegion[PlayerRegion["Americas"] = 2] = "Americas"
    PlayerRegion[PlayerRegion["Asia"] = 3] = "Asia"
    PlayerRegion[PlayerRegion["Australasia"] = 4] = "Australasia"
    PlayerRegion[PlayerRegion["Europe"] = 5] = "Europe"
})(PlayerRegion || (PlayerRegion = {}));
export class AuditBase {
    /** @param {{createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    createdDate;
    /** @type {string} */
    createdBy;
    /** @type {string} */
    modifiedDate;
    /** @type {string} */
    modifiedBy;
    /** @type {?string} */
    deletedDate;
    /** @type {string} */
    deletedBy;
}
export class Profile extends AuditBase {
    /** @param {{id?:number,role?:PlayerRole,region?:PlayerRegion,username?:string,highScore?:number,gamesPlayed?:number,energy?:number,profileUrl?:string,coverUrl?:string,meta?:{ [index: string]: string; },createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {PlayerRole} */
    role;
    /** @type {PlayerRegion} */
    region;
    /** @type {?string} */
    username;
    /** @type {number} */
    highScore;
    /** @type {number} */
    gamesPlayed;
    /** @type {number} */
    energy;
    /** @type {?string} */
    profileUrl;
    /** @type {?string} */
    coverUrl;
    /** @type {?{ [index: string]: string; }} */
    meta;
}
export class GameItem extends AuditBase {
    /** @param {{name?:string,imageUrl?:string,description?:string,dateAdded?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {string} */
    name;
    /** @type {string} */
    imageUrl;
    /** @type {?string} */
    description;
    /** @type {string} */
    dateAdded;
}
/** @typedef {'FullTime'|'PartTime'|'Casual'|'Contract'} */
export var EmploymentType;
(function (EmploymentType) {
    EmploymentType["FullTime"] = "FullTime"
    EmploymentType["PartTime"] = "PartTime"
    EmploymentType["Casual"] = "Casual"
    EmploymentType["Contract"] = "Contract"
})(EmploymentType || (EmploymentType = {}));
export class Job extends AuditBase {
    /** @param {{id?:number,title?:string,employmentType?:EmploymentType,company?:string,location?:string,salaryRangeLower?:number,salaryRangeUpper?:number,description?:string,applications?:JobApplication[],closing?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    title;
    /** @type {EmploymentType} */
    employmentType;
    /** @type {string} */
    company;
    /** @type {string} */
    location;
    /** @type {number} */
    salaryRangeLower;
    /** @type {number} */
    salaryRangeUpper;
    /** @type {string} */
    description;
    /** @type {JobApplication[]} */
    applications;
    /** @type {string} */
    closing;
}
export class Contact {
    /** @param {{id?:number,displayName?:string,profileUrl?:string,firstName?:string,lastName?:string,salaryExpectation?:number,jobType?:string,availabilityWeeks?:number,preferredWorkType?:EmploymentType,preferredLocation?:string,email?:string,phone?:string,skills?:string[],about?:string,applications?:JobApplication[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    displayName;
    /** @type {string} */
    profileUrl;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {?number} */
    salaryExpectation;
    /** @type {string} */
    jobType;
    /** @type {number} */
    availabilityWeeks;
    /** @type {EmploymentType} */
    preferredWorkType;
    /** @type {string} */
    preferredLocation;
    /** @type {string} */
    email;
    /** @type {string} */
    phone;
    /** @type {?string[]} */
    skills;
    /** @type {string} */
    about;
    /** @type {JobApplication[]} */
    applications;
}
/** @typedef {'None'|'Marketing'|'Accounts'|'Legal'|'HumanResources'} */
export var Department;
(function (Department) {
    Department["None"] = "None"
    Department["Marketing"] = "Marketing"
    Department["Accounts"] = "Accounts"
    Department["Legal"] = "Legal"
    Department["HumanResources"] = "HumanResources"
})(Department || (Department = {}));
export class AppUser {
    /** @param {{id?:number,displayName?:string,email?:string,profileUrl?:string,department?:Department,title?:string,jobArea?:string,location?:string,salary?:number,about?:string,isArchived?:boolean,archivedDate?:string,lastLoginDate?:string,lastLoginIp?:string,userName?:string,primaryEmail?:string,firstName?:string,lastName?:string,company?:string,country?:string,phoneNumber?:string,birthDate?:string,birthDateRaw?:string,address?:string,address2?:string,city?:string,state?:string,culture?:string,fullName?:string,gender?:string,language?:string,mailAddress?:string,nickname?:string,postalCode?:string,timeZone?:string,salt?:string,passwordHash?:string,digestHa1Hash?:string,roles?:string[],permissions?:string[],createdDate?:string,modifiedDate?:string,invalidLoginAttempts?:number,lastLoginAttempt?:string,lockedDate?:string,recoveryToken?:string,refId?:number,refIdStr?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    displayName;
    /** @type {string} */
    email;
    /** @type {string} */
    profileUrl;
    /** @type {Department} */
    department;
    /** @type {string} */
    title;
    /** @type {string} */
    jobArea;
    /** @type {string} */
    location;
    /** @type {number} */
    salary;
    /** @type {string} */
    about;
    /** @type {boolean} */
    isArchived;
    /** @type {?string} */
    archivedDate;
    /** @type {?string} */
    lastLoginDate;
    /** @type {string} */
    lastLoginIp;
    /** @type {string} */
    userName;
    /** @type {string} */
    primaryEmail;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    company;
    /** @type {string} */
    country;
    /** @type {string} */
    phoneNumber;
    /** @type {?string} */
    birthDate;
    /** @type {string} */
    birthDateRaw;
    /** @type {string} */
    address;
    /** @type {string} */
    address2;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    culture;
    /** @type {string} */
    fullName;
    /** @type {string} */
    gender;
    /** @type {string} */
    language;
    /** @type {string} */
    mailAddress;
    /** @type {string} */
    nickname;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    timeZone;
    /** @type {string} */
    salt;
    /** @type {string} */
    passwordHash;
    /** @type {string} */
    digestHa1Hash;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {string} */
    createdDate;
    /** @type {string} */
    modifiedDate;
    /** @type {number} */
    invalidLoginAttempts;
    /** @type {?string} */
    lastLoginAttempt;
    /** @type {?string} */
    lockedDate;
    /** @type {string} */
    recoveryToken;
    /** @type {?number} */
    refId;
    /** @type {string} */
    refIdStr;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class JobApplicationComment extends AuditBase {
    /** @param {{id?:number,appUserId?:number,appUser?:AppUser,jobApplicationId?:number,comment?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    appUserId;
    /** @type {AppUser} */
    appUser;
    /** @type {number} */
    jobApplicationId;
    /** @type {string} */
    comment;
}
/** @typedef {'Applied'|'PhoneScreening'|'PhoneScreeningCompleted'|'Interview'|'InterviewCompleted'|'Offer'|'Disqualified'} */
export var JobApplicationStatus;
(function (JobApplicationStatus) {
    JobApplicationStatus["Applied"] = "Applied"
    JobApplicationStatus["PhoneScreening"] = "PhoneScreening"
    JobApplicationStatus["PhoneScreeningCompleted"] = "PhoneScreeningCompleted"
    JobApplicationStatus["Interview"] = "Interview"
    JobApplicationStatus["InterviewCompleted"] = "InterviewCompleted"
    JobApplicationStatus["Offer"] = "Offer"
    JobApplicationStatus["Disqualified"] = "Disqualified"
})(JobApplicationStatus || (JobApplicationStatus = {}));
export class JobApplicationAttachment {
    /** @param {{id?:number,jobApplicationId?:number,fileName?:string,filePath?:string,contentType?:string,contentLength?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    jobApplicationId;
    /** @type {string} */
    fileName;
    /** @type {string} */
    filePath;
    /** @type {string} */
    contentType;
    /** @type {number} */
    contentLength;
}
export class JobApplicationEvent extends AuditBase {
    /** @param {{id?:number,jobApplicationId?:number,appUserId?:number,appUser?:AppUser,description?:string,status?:JobApplicationStatus,eventDate?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    jobApplicationId;
    /** @type {number} */
    appUserId;
    /** @type {AppUser} */
    appUser;
    /** @type {string} */
    description;
    /** @type {?JobApplicationStatus} */
    status;
    /** @type {string} */
    eventDate;
}
export class PhoneScreen extends AuditBase {
    /** @param {{id?:number,appUserId?:number,appUser?:AppUser,jobApplicationId?:number,applicationStatus?:JobApplicationStatus,notes?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    appUserId;
    /** @type {AppUser} */
    appUser;
    /** @type {number} */
    jobApplicationId;
    /** @type {?JobApplicationStatus} */
    applicationStatus;
    /** @type {string} */
    notes;
}
export class Interview extends AuditBase {
    /** @param {{id?:number,bookingTime?:string,jobApplicationId?:number,appUserId?:number,appUser?:AppUser,applicationStatus?:JobApplicationStatus,notes?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    bookingTime;
    /** @type {number} */
    jobApplicationId;
    /** @type {number} */
    appUserId;
    /** @type {AppUser} */
    appUser;
    /** @type {?JobApplicationStatus} */
    applicationStatus;
    /** @type {string} */
    notes;
}
export class JobOffer extends AuditBase {
    /** @param {{id?:number,salaryOffer?:number,currency?:string,jobApplicationId?:number,appUserId?:number,appUser?:AppUser,notes?:string,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    salaryOffer;
    /** @type {string} */
    currency;
    /** @type {number} */
    jobApplicationId;
    /** @type {number} */
    appUserId;
    /** @type {AppUser} */
    appUser;
    /** @type {string} */
    notes;
}
export class JobApplication {
    /** @param {{id?:number,jobId?:number,contactId?:number,position?:Job,applicant?:Contact,comments?:JobApplicationComment[],appliedDate?:string,applicationStatus?:JobApplicationStatus,attachments?:JobApplicationAttachment[],events?:JobApplicationEvent[],phoneScreen?:PhoneScreen,interview?:Interview,jobOffer?:JobOffer}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    jobId;
    /** @type {number} */
    contactId;
    /** @type {Job} */
    position;
    /** @type {Contact} */
    applicant;
    /** @type {JobApplicationComment[]} */
    comments;
    /** @type {string} */
    appliedDate;
    /** @type {JobApplicationStatus} */
    applicationStatus;
    /** @type {JobApplicationAttachment[]} */
    attachments;
    /** @type {JobApplicationEvent[]} */
    events;
    /** @type {PhoneScreen} */
    phoneScreen;
    /** @type {Interview} */
    interview;
    /** @type {JobOffer} */
    jobOffer;
}
/** @typedef T {any} */
export class QueryDb extends QueryBase {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
}
export class Albums {
    /** @param {{albumId?:number,title?:string,artistId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    albumId;
    /** @type {string} */
    title;
    /** @type {number} */
    artistId;
}
export class Artists {
    /** @param {{artistId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    artistId;
    /** @type {string} */
    name;
}
export class Customers {
    /** @param {{customerId?:number,firstName?:string,lastName?:string,company?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string,supportRepId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    customerId;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    company;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    /** @type {?number} */
    supportRepId;
}
export class Employees {
    /** @param {{employeeId?:number,lastName?:string,firstName?:string,title?:string,reportsTo?:number,birthDate?:string,hireDate?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    employeeId;
    /** @type {string} */
    lastName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    title;
    /** @type {?number} */
    reportsTo;
    /** @type {?string} */
    birthDate;
    /** @type {?string} */
    hireDate;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
}
export class Genres {
    /** @param {{genreId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    genreId;
    /** @type {string} */
    name;
}
export class InvoiceItems {
    /** @param {{invoiceLineId?:number,invoiceId?:number,trackId?:number,unitPrice?:number,quantity?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceLineId;
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    trackId;
    /** @type {number} */
    unitPrice;
    /** @type {number} */
    quantity;
}
export class Invoices {
    /** @param {{invoiceId?:number,customerId?:number,invoiceDate?:string,billingAddress?:string,billingCity?:string,billingState?:string,billingCountry?:string,billingPostalCode?:string,total?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    customerId;
    /** @type {string} */
    invoiceDate;
    /** @type {string} */
    billingAddress;
    /** @type {string} */
    billingCity;
    /** @type {string} */
    billingState;
    /** @type {string} */
    billingCountry;
    /** @type {string} */
    billingPostalCode;
    /** @type {number} */
    total;
}
export class MediaTypes {
    /** @param {{mediaTypeId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    mediaTypeId;
    /** @type {string} */
    name;
}
export class Playlists {
    /** @param {{playlistId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    playlistId;
    /** @type {string} */
    name;
}
export class Tracks {
    /** @param {{trackId?:number,name?:string,albumId?:number,mediaTypeId?:number,genreId?:number,composer?:string,milliseconds?:number,bytes?:number,unitPrice?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    trackId;
    /** @type {string} */
    name;
    /** @type {?number} */
    albumId;
    /** @type {number} */
    mediaTypeId;
    /** @type {?number} */
    genreId;
    /** @type {string} */
    composer;
    /** @type {number} */
    milliseconds;
    /** @type {?number} */
    bytes;
    /** @type {number} */
    unitPrice;
}
/** @typedef {'Single'|'Double'|'Queen'|'Twin'|'Suite'} */
export var RoomType;
(function (RoomType) {
    RoomType["Single"] = "Single"
    RoomType["Double"] = "Double"
    RoomType["Queen"] = "Queen"
    RoomType["Twin"] = "Twin"
    RoomType["Suite"] = "Suite"
})(RoomType || (RoomType = {}));
export class Coupon {
    /** @param {{id?:string,description?:string,discount?:number,expiryDate?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    id;
    /** @type {string} */
    description;
    /** @type {number} */
    discount;
    /** @type {string} */
    expiryDate;
}
export class Booking extends AuditBase {
    /** @param {{id?:number,name?:string,roomType?:RoomType,roomNumber?:number,bookingStartDate?:string,bookingEndDate?:string,cost?:number,couponId?:string,discount?:Coupon,notes?:string,cancelled?:boolean,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    name;
    /** @type {RoomType} */
    roomType;
    /** @type {number} */
    roomNumber;
    /** @type {string} */
    bookingStartDate;
    /** @type {?string} */
    bookingEndDate;
    /** @type {number} */
    cost;
    /** @type {?string} */
    couponId;
    /** @type {Coupon} */
    discount;
    /** @type {?string} */
    notes;
    /** @type {?boolean} */
    cancelled;
}
/** @typedef {'Public'|'Team'|'Private'} */
export var FileAccessType;
(function (FileAccessType) {
    FileAccessType["Public"] = "Public"
    FileAccessType["Team"] = "Team"
    FileAccessType["Private"] = "Private"
})(FileAccessType || (FileAccessType = {}));
export class FileSystemFile {
    /** @param {{id?:number,fileName?:string,filePath?:string,contentType?:string,contentLength?:number,fileSystemItemId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    fileName;
    /** @type {string} */
    filePath;
    /** @type {string} */
    contentType;
    /** @type {number} */
    contentLength;
    /** @type {number} */
    fileSystemItemId;
}
/** @typedef {'Home'|'Mobile'|'Work'} */
export var PhoneKind;
(function (PhoneKind) {
    PhoneKind["Home"] = "Home"
    PhoneKind["Mobile"] = "Mobile"
    PhoneKind["Work"] = "Work"
})(PhoneKind || (PhoneKind = {}));
export class Phone {
    /** @param {{kind?:PhoneKind,number?:string,ext?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {PhoneKind} */
    kind;
    /** @type {string} */
    number;
    /** @type {string} */
    ext;
}
export class PlayerGameItem {
    /** @param {{id?:number,playerId?:number,gameItemName?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {number} */
    playerId;
    /** @type {string} */
    gameItemName;
}
export class Player extends AuditBase {
    /** @param {{id?:number,firstName?:string,lastName?:string,email?:string,phoneNumbers?:Phone[],gameItems?:PlayerGameItem[],profile?:Profile,profileId?:number,savedLevelId?:string,rowVersion?:number,createdDate?:string,createdBy?:string,modifiedDate?:string,modifiedBy?:string,deletedDate?:string,deletedBy?:string}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    email;
    /** @type {Phone[]} */
    phoneNumbers;
    /** @type {PlayerGameItem[]} */
    gameItems;
    /** @type {Profile} */
    profile;
    /** @type {number} */
    profileId;
    /** @type {string} */
    savedLevelId;
    /** @type {number} */
    rowVersion;
}
export class Level {
    /** @param {{id?:string,data?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    id;
    /** @type {string} */
    data;
}
export class FileSystemItem {
    /** @param {{id?:number,fileAccessType?:FileAccessType,file?:FileSystemFile,appUserId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?FileAccessType} */
    fileAccessType;
    /** @type {FileSystemFile} */
    file;
    /** @type {number} */
    appUserId;
}
export class ResponseError {
    /** @param {{errorCode?:string,fieldName?:string,message?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    errorCode;
    /** @type {string} */
    fieldName;
    /** @type {string} */
    message;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class ResponseStatus {
    /** @param {{errorCode?:string,message?:string,stackTrace?:string,errors?:ResponseError[],meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    errorCode;
    /** @type {string} */
    message;
    /** @type {string} */
    stackTrace;
    /** @type {ResponseError[]} */
    errors;
    /** @type {{ [index: string]: string; }} */
    meta;
}
/** @typedef TKey {any} */
/** @typedef  TValue {any} */
export class KeyValuePair {
    /** @param {{key?:TKey,value?:TValue}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {TKey} */
    key;
    /** @type {TValue} */
    value;
}
export class HelloAllTypesResponse {
    /** @param {{result?:string,allTypes?:AllTypes,allCollectionTypes?:AllCollectionTypes}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    result;
    /** @type {AllTypes} */
    allTypes;
    /** @type {AllCollectionTypes} */
    allCollectionTypes;
}
export class ComboBoxExamples {
    /** @param {{singleClientValues?:string,multipleClientValues?:string[],singleServerValues?:string,multipleServerValues?:string[],singleServerEntries?:string,multipleServerEntries?:string[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {?string} */
    singleClientValues;
    /** @type {?string[]} */
    multipleClientValues;
    /** @type {?string} */
    singleServerValues;
    /** @type {?string[]} */
    multipleServerValues;
    /** @type {?string} */
    singleServerEntries;
    /** @type {?string[]} */
    multipleServerEntries;
    getTypeName() { return 'ComboBoxExamples' }
    getMethod() { return 'POST' }
    createResponse() { return new ComboBoxExamples() }
}
export class HelloResponse {
    /** @param {{result?:string,responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    result;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class Todo {
    /** @param {{id?:number,text?:string,isFinished?:boolean}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    text;
    /** @type {boolean} */
    isFinished;
}
/** @typedef T {any} */
export class QueryResponse {
    /** @param {{offset?:number,total?:number,results?:T[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    offset;
    /** @type {number} */
    total;
    /** @type {T[]} */
    results;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class AuthenticateResponse {
    /** @param {{userId?:string,sessionId?:string,userName?:string,displayName?:string,referrerUrl?:string,bearerToken?:string,refreshToken?:string,profileUrl?:string,roles?:string[],permissions?:string[],responseStatus?:ResponseStatus,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userId;
    /** @type {string} */
    sessionId;
    /** @type {string} */
    userName;
    /** @type {string} */
    displayName;
    /** @type {string} */
    referrerUrl;
    /** @type {string} */
    bearerToken;
    /** @type {string} */
    refreshToken;
    /** @type {string} */
    profileUrl;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {ResponseStatus} */
    responseStatus;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class AssignRolesResponse {
    /** @param {{allRoles?:string[],allPermissions?:string[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string[]} */
    allRoles;
    /** @type {string[]} */
    allPermissions;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class UnAssignRolesResponse {
    /** @param {{allRoles?:string[],allPermissions?:string[],meta?:{ [index: string]: string; },responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string[]} */
    allRoles;
    /** @type {string[]} */
    allPermissions;
    /** @type {{ [index: string]: string; }} */
    meta;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class RegisterResponse {
    /** @param {{userId?:string,sessionId?:string,userName?:string,referrerUrl?:string,bearerToken?:string,refreshToken?:string,roles?:string[],permissions?:string[],responseStatus?:ResponseStatus,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userId;
    /** @type {string} */
    sessionId;
    /** @type {string} */
    userName;
    /** @type {string} */
    referrerUrl;
    /** @type {string} */
    bearerToken;
    /** @type {string} */
    refreshToken;
    /** @type {string[]} */
    roles;
    /** @type {string[]} */
    permissions;
    /** @type {ResponseStatus} */
    responseStatus;
    /** @type {{ [index: string]: string; }} */
    meta;
}
export class IdResponse {
    /** @param {{id?:string,responseStatus?:ResponseStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    id;
    /** @type {ResponseStatus} */
    responseStatus;
}
export class HelloAllTypes {
    /** @param {{name?:string,allTypes?:AllTypes,allCollectionTypes?:AllCollectionTypes}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    /** @type {AllTypes} */
    allTypes;
    /** @type {AllCollectionTypes} */
    allCollectionTypes;
    getTypeName() { return 'HelloAllTypes' }
    getMethod() { return 'POST' }
    createResponse() { return new HelloAllTypesResponse() }
}
export class Hello {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'Hello' }
    getMethod() { return 'POST' }
    createResponse() { return new HelloResponse() }
}
export class HelloSecure {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'HelloSecure' }
    getMethod() { return 'POST' }
    createResponse() { return new HelloResponse() }
}
export class QueryTodos extends QueryData {
    /** @param {{id?:number,ids?:number[],textContains?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number[]} */
    ids;
    /** @type {?string} */
    textContains;
    getTypeName() { return 'QueryTodos' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class CreateTodo {
    /** @param {{text?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    text;
    getTypeName() { return 'CreateTodo' }
    getMethod() { return 'POST' }
    createResponse() { return new Todo() }
}
export class UpdateTodo {
    /** @param {{id?:number,text?:string,isFinished?:boolean}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    text;
    /** @type {boolean} */
    isFinished;
    getTypeName() { return 'UpdateTodo' }
    getMethod() { return 'PUT' }
    createResponse() { return new Todo() }
}
export class DeleteTodo {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteTodo' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class DeleteTodos {
    /** @param {{ids?:number[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number[]} */
    ids;
    getTypeName() { return 'DeleteTodos' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class Authenticate {
    /** @param {{provider?:string,state?:string,oauth_token?:string,oauth_verifier?:string,userName?:string,password?:string,rememberMe?:boolean,errorView?:string,nonce?:string,uri?:string,response?:string,qop?:string,nc?:string,cnonce?:string,accessToken?:string,accessTokenSecret?:string,scope?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /**
     * @type {string}
     * @description AuthProvider, e.g. credentials */
    provider;
    /** @type {string} */
    state;
    /** @type {string} */
    oauth_token;
    /** @type {string} */
    oauth_verifier;
    /** @type {string} */
    userName;
    /** @type {string} */
    password;
    /** @type {?boolean} */
    rememberMe;
    /** @type {string} */
    errorView;
    /** @type {string} */
    nonce;
    /** @type {string} */
    uri;
    /** @type {string} */
    response;
    /** @type {string} */
    qop;
    /** @type {string} */
    nc;
    /** @type {string} */
    cnonce;
    /** @type {string} */
    accessToken;
    /** @type {string} */
    accessTokenSecret;
    /** @type {string} */
    scope;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'Authenticate' }
    getMethod() { return 'POST' }
    createResponse() { return new AuthenticateResponse() }
}
export class AssignRoles {
    /** @param {{userName?:string,permissions?:string[],roles?:string[],meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string[]} */
    permissions;
    /** @type {string[]} */
    roles;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'AssignRoles' }
    getMethod() { return 'POST' }
    createResponse() { return new AssignRolesResponse() }
}
export class UnAssignRoles {
    /** @param {{userName?:string,permissions?:string[],roles?:string[],meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string[]} */
    permissions;
    /** @type {string[]} */
    roles;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'UnAssignRoles' }
    getMethod() { return 'POST' }
    createResponse() { return new UnAssignRolesResponse() }
}
export class Register {
    /** @param {{userName?:string,firstName?:string,lastName?:string,displayName?:string,email?:string,password?:string,confirmPassword?:string,autoLogin?:boolean,errorView?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    userName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    displayName;
    /** @type {string} */
    email;
    /** @type {string} */
    password;
    /** @type {string} */
    confirmPassword;
    /** @type {?boolean} */
    autoLogin;
    /** @type {string} */
    errorView;
    /** @type {{ [index: string]: string; }} */
    meta;
    getTypeName() { return 'Register' }
    getMethod() { return 'POST' }
    createResponse() { return new RegisterResponse() }
}
export class CreateProfile {
    /** @param {{role?:PlayerRole,region?:PlayerRegion,username?:string,highScore?:number,gamesPlayed?:number,energy?:number,profileUrl?:string,coverUrl?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {PlayerRole} */
    role;
    /** @type {PlayerRegion} */
    region;
    /** @type {string} */
    username;
    /** @type {number} */
    highScore;
    /** @type {number} */
    gamesPlayed;
    /** @type {number} */
    energy;
    /** @type {?string} */
    profileUrl;
    /** @type {?string} */
    coverUrl;
    getTypeName() { return 'CreateProfile' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class UpdateProfile {
    /** @param {{id?:number,role?:PlayerRole,region?:PlayerRegion,username?:string,highScore?:number,gamesPlayed?:number,energy?:number,profileUrl?:string,coverUrl?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?PlayerRole} */
    role;
    /** @type {?PlayerRegion} */
    region;
    /** @type {?string} */
    username;
    /** @type {?number} */
    highScore;
    /** @type {?number} */
    gamesPlayed;
    /** @type {?number} */
    energy;
    /** @type {?string} */
    profileUrl;
    /** @type {?string} */
    coverUrl;
    getTypeName() { return 'UpdateProfile' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class DeleteProfile {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteProfile' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreateGameItem {
    /** @param {{name?:string,description?:string,imageUrl?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    /** @type {string} */
    description;
    /** @type {string} */
    imageUrl;
    getTypeName() { return 'CreateGameItem' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class UpdateGameItem {
    /** @param {{name?:string,description?:string,imageUrl?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    /** @type {string} */
    description;
    /** @type {?string} */
    imageUrl;
    getTypeName() { return 'UpdateGameItem' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class DeleteGameItem {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'DeleteGameItem' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreateContact {
    /** @param {{firstName?:string,lastName?:string,profileUrl?:string,salaryExpectation?:number,jobType?:string,availabilityWeeks?:number,preferredWorkType?:EmploymentType,preferredLocation?:string,email?:string,phone?:string,skills?:string[],about?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {?string} */
    profileUrl;
    /** @type {?number} */
    salaryExpectation;
    /** @type {string} */
    jobType;
    /** @type {number} */
    availabilityWeeks;
    /** @type {EmploymentType} */
    preferredWorkType;
    /** @type {string} */
    preferredLocation;
    /** @type {string} */
    email;
    /** @type {?string} */
    phone;
    /** @type {?string[]} */
    skills;
    /** @type {?string} */
    about;
    getTypeName() { return 'CreateContact' }
    getMethod() { return 'POST' }
    createResponse() { return new Contact() }
}
export class UpdateContact {
    /** @param {{id?:number,firstName?:string,lastName?:string,profileUrl?:string,salaryExpectation?:number,jobType?:string,availabilityWeeks?:number,preferredWorkType?:EmploymentType,preferredLocation?:string,email?:string,phone?:string,skills?:string[],about?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {?string} */
    profileUrl;
    /** @type {?number} */
    salaryExpectation;
    /** @type {string} */
    jobType;
    /** @type {?number} */
    availabilityWeeks;
    /** @type {?EmploymentType} */
    preferredWorkType;
    /** @type {?string} */
    preferredLocation;
    /** @type {string} */
    email;
    /** @type {?string} */
    phone;
    /** @type {?string[]} */
    skills;
    /** @type {?string} */
    about;
    getTypeName() { return 'UpdateContact' }
    getMethod() { return 'PATCH' }
    createResponse() { return new Contact() }
}
export class DeleteContact {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteContact' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreateJob {
    /** @param {{title?:string,salaryRangeLower?:number,salaryRangeUpper?:number,description?:string,employmentType?:EmploymentType,company?:string,location?:string,closing?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    title;
    /** @type {number} */
    salaryRangeLower;
    /** @type {number} */
    salaryRangeUpper;
    /** @type {string} */
    description;
    /** @type {EmploymentType} */
    employmentType;
    /** @type {string} */
    company;
    /** @type {string} */
    location;
    /** @type {string} */
    closing;
    getTypeName() { return 'CreateJob' }
    getMethod() { return 'POST' }
    createResponse() { return new Job() }
}
export class UpdateJob {
    /** @param {{id?:number,title?:string,salaryRangeLower?:number,salaryRangeUpper?:number,description?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?string} */
    title;
    /** @type {?number} */
    salaryRangeLower;
    /** @type {?number} */
    salaryRangeUpper;
    /** @type {?string} */
    description;
    getTypeName() { return 'UpdateJob' }
    getMethod() { return 'PATCH' }
    createResponse() { return new Job() }
}
export class DeleteJob {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteJob' }
    getMethod() { return 'DELETE' }
    createResponse() { return new Job() }
}
export class CreateJobApplication {
    /** @param {{jobId?:number,contactId?:number,appliedDate?:string,applicationStatus?:JobApplicationStatus,attachments?:JobApplicationAttachment[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    jobId;
    /** @type {number} */
    contactId;
    /** @type {string} */
    appliedDate;
    /** @type {JobApplicationStatus} */
    applicationStatus;
    /** @type {JobApplicationAttachment[]} */
    attachments;
    getTypeName() { return 'CreateJobApplication' }
    getMethod() { return 'POST' }
    createResponse() { return new JobApplication() }
}
export class UpdateJobApplication {
    /** @param {{id?:number,jobId?:number,contactId?:number,appliedDate?:string,applicationStatus?:JobApplicationStatus,attachments?:JobApplicationAttachment[]}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?number} */
    jobId;
    /** @type {?number} */
    contactId;
    /** @type {?string} */
    appliedDate;
    /** @type {?JobApplicationStatus} */
    applicationStatus;
    /** @type {?JobApplicationAttachment[]} */
    attachments;
    getTypeName() { return 'UpdateJobApplication' }
    getMethod() { return 'PATCH' }
    createResponse() { return new JobApplication() }
}
export class DeleteJobApplication {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteJobApplication' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreatePhoneScreen {
    /** @param {{jobApplicationId?:number,appUserId?:number,applicationStatus?:JobApplicationStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    jobApplicationId;
    /** @type {number} */
    appUserId;
    /** @type {JobApplicationStatus} */
    applicationStatus;
    getTypeName() { return 'CreatePhoneScreen' }
    getMethod() { return 'POST' }
    createResponse() { return new PhoneScreen() }
}
export class UpdatePhoneScreen {
    /** @param {{id?:number,jobApplicationId?:number,notes?:string,applicationStatus?:JobApplicationStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?number} */
    jobApplicationId;
    /** @type {?string} */
    notes;
    /** @type {?JobApplicationStatus} */
    applicationStatus;
    getTypeName() { return 'UpdatePhoneScreen' }
    getMethod() { return 'PATCH' }
    createResponse() { return new PhoneScreen() }
}
export class CreateInterview {
    /** @param {{bookingTime?:string,jobApplicationId?:number,appUserId?:number,applicationStatus?:JobApplicationStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    bookingTime;
    /** @type {number} */
    jobApplicationId;
    /** @type {number} */
    appUserId;
    /** @type {JobApplicationStatus} */
    applicationStatus;
    getTypeName() { return 'CreateInterview' }
    getMethod() { return 'POST' }
    createResponse() { return new Interview() }
}
export class UpdateInterview {
    /** @param {{id?:number,jobApplicationId?:number,notes?:string,applicationStatus?:JobApplicationStatus}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?number} */
    jobApplicationId;
    /** @type {?string} */
    notes;
    /** @type {?JobApplicationStatus} */
    applicationStatus;
    getTypeName() { return 'UpdateInterview' }
    getMethod() { return 'PATCH' }
    createResponse() { return new Interview() }
}
export class CreateJobOffer {
    /** @param {{salaryOffer?:number,currency?:string,jobApplicationId?:number,applicationStatus?:JobApplicationStatus,notes?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    salaryOffer;
    /** @type {string} */
    currency;
    /** @type {number} */
    jobApplicationId;
    /** @type {JobApplicationStatus} */
    applicationStatus;
    /** @type {string} */
    notes;
    getTypeName() { return 'CreateJobOffer' }
    getMethod() { return 'POST' }
    createResponse() { return new JobOffer() }
}
export class UpdateJobOffer {
    /** @param {{id?:number,salaryOffer?:number,currency?:string,jobApplicationId?:number,applicationStatus?:JobApplicationStatus,notes?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number} */
    salaryOffer;
    /** @type {string} */
    currency;
    /** @type {?number} */
    jobApplicationId;
    /** @type {?JobApplicationStatus} */
    applicationStatus;
    /** @type {?string} */
    notes;
    getTypeName() { return 'UpdateJobOffer' }
    getMethod() { return 'PATCH' }
    createResponse() { return new JobOffer() }
}
export class CreateJobApplicationEvent {
    constructor(init) { Object.assign(this, init) }
    getTypeName() { return 'CreateJobApplicationEvent' }
    getMethod() { return 'POST' }
    createResponse() { return new JobApplicationEvent() }
}
export class UpdateJobApplicationEvent {
    /** @param {{id?:number,status?:JobApplicationStatus,description?:string,eventDate?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?JobApplicationStatus} */
    status;
    /** @type {?string} */
    description;
    /** @type {?string} */
    eventDate;
    getTypeName() { return 'UpdateJobApplicationEvent' }
    getMethod() { return 'PATCH' }
    createResponse() { return new JobApplicationEvent() }
}
export class DeleteJobApplicationEvent {
    constructor(init) { Object.assign(this, init) }
    getTypeName() { return 'DeleteJobApplicationEvent' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreateJobApplicationComment {
    /** @param {{jobApplicationId?:number,comment?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    jobApplicationId;
    /** @type {string} */
    comment;
    getTypeName() { return 'CreateJobApplicationComment' }
    getMethod() { return 'POST' }
    createResponse() { return new JobApplicationComment() }
}
export class UpdateJobApplicationComment {
    /** @param {{id?:number,jobApplicationId?:number,comment?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?number} */
    jobApplicationId;
    /** @type {?string} */
    comment;
    getTypeName() { return 'UpdateJobApplicationComment' }
    getMethod() { return 'PATCH' }
    createResponse() { return new JobApplicationComment() }
}
export class DeleteJobApplicationComment {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteJobApplicationComment' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class QueryAlbums extends QueryDb {
    /** @param {{albumId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    albumId;
    getTypeName() { return 'QueryAlbums' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryArtists extends QueryDb {
    /** @param {{artistId?:number,artistIdBetween?:number[],nameStartsWith?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    artistId;
    /** @type {number[]} */
    artistIdBetween;
    /** @type {string} */
    nameStartsWith;
    getTypeName() { return 'QueryArtists' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryChinookCustomers extends QueryDb {
    /** @param {{customerId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    customerId;
    getTypeName() { return 'QueryChinookCustomers' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryChinookEmployees extends QueryDb {
    /** @param {{employeeId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    employeeId;
    getTypeName() { return 'QueryChinookEmployees' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryGenres extends QueryDb {
    /** @param {{genreId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    genreId;
    getTypeName() { return 'QueryGenres' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryInvoiceItems extends QueryDb {
    /** @param {{invoiceLineId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    invoiceLineId;
    getTypeName() { return 'QueryInvoiceItems' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryInvoices extends QueryDb {
    /** @param {{invoiceId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    invoiceId;
    getTypeName() { return 'QueryInvoices' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryMediaTypes extends QueryDb {
    /** @param {{mediaTypeId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    mediaTypeId;
    getTypeName() { return 'QueryMediaTypes' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryPlaylists extends QueryDb {
    /** @param {{playlistId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    playlistId;
    getTypeName() { return 'QueryPlaylists' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryTracks extends QueryDb {
    /** @param {{trackId?:number,nameContains?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    trackId;
    /** @type {string} */
    nameContains;
    getTypeName() { return 'QueryTracks' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryBookings extends QueryDb {
    /** @param {{id?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    getTypeName() { return 'QueryBookings' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryCoupons extends QueryDb {
    /** @param {{id?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {string} */
    id;
    getTypeName() { return 'QueryCoupons' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryFileSystemItems extends QueryDb {
    /** @param {{appUserId?:number,fileAccessType?:FileAccessType,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    appUserId;
    /** @type {?FileAccessType} */
    fileAccessType;
    getTypeName() { return 'QueryFileSystemItems' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryFileSystemFiles extends QueryDb {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    getTypeName() { return 'QueryFileSystemFiles' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryPlayer extends QueryDb {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    getTypeName() { return 'QueryPlayer' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryProfile extends QueryDb {
    /** @param {{skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    getTypeName() { return 'QueryProfile' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryGameItem extends QueryDb {
    /** @param {{name?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'QueryGameItem' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryPlayerGameItem extends QueryDb {
    /** @param {{id?:number,playerId?:number,gameItemName?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number} */
    playerId;
    /** @type {?string} */
    gameItemName;
    getTypeName() { return 'QueryPlayerGameItem' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryLevel extends QueryDb {
    /** @param {{id?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?string} */
    id;
    getTypeName() { return 'QueryLevel' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryJobApplicationAttachments extends QueryDb {
    /** @param {{id?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    getTypeName() { return 'QueryJobApplicationAttachments' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryContacts extends QueryDb {
    /** @param {{id?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    getTypeName() { return 'QueryContacts' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryJobs extends QueryDb {
    /** @param {{id?:number,ids?:number[],skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number[]} */
    ids;
    getTypeName() { return 'QueryJobs' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryJobApplications extends QueryDb {
    /** @param {{id?:number,ids?:number[],jobId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number[]} */
    ids;
    /** @type {?number} */
    jobId;
    getTypeName() { return 'QueryJobApplications' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryPhoneScreens extends QueryDb {
    /** @param {{id?:number,jobApplicationId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number} */
    jobApplicationId;
    getTypeName() { return 'QueryPhoneScreens' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryInterviews extends QueryDb {
    /** @param {{id?:number,jobApplicationId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number} */
    jobApplicationId;
    getTypeName() { return 'QueryInterviews' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryJobOffers extends QueryDb {
    /** @param {{id?:number,jobApplicationId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    id;
    /** @type {?number} */
    jobApplicationId;
    getTypeName() { return 'QueryJobOffers' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryJobApplicationEvents extends QueryDb {
    /** @param {{jobApplicationId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    jobApplicationId;
    getTypeName() { return 'QueryJobApplicationEvents' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryAppUsers extends QueryDb {
    /** @param {{emailContains?:string,firstNameContains?:string,lastNameContains?:string,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?string} */
    emailContains;
    /** @type {?string} */
    firstNameContains;
    /** @type {?string} */
    lastNameContains;
    getTypeName() { return 'QueryAppUsers' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class QueryJobApplicationComments extends QueryDb {
    /** @param {{jobApplicationId?:number,skip?:number,take?:number,orderBy?:string,orderByDesc?:string,include?:string,fields?:string,meta?:{ [index: string]: string; }}} [init] */
    constructor(init) { super(init); Object.assign(this, init) }
    /** @type {?number} */
    jobApplicationId;
    getTypeName() { return 'QueryJobApplicationComments' }
    getMethod() { return 'GET' }
    createResponse() { return new QueryResponse() }
}
export class CreateAlbums {
    /** @param {{title?:string,artistId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    title;
    /** @type {number} */
    artistId;
    getTypeName() { return 'CreateAlbums' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateArtists {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'CreateArtists' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateChinookCustomer {
    /** @param {{firstName?:string,lastName?:string,company?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string,supportRepId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    company;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    /** @type {?number} */
    supportRepId;
    getTypeName() { return 'CreateChinookCustomer' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateChinookEmployee {
    /** @param {{lastName?:string,firstName?:string,title?:string,reportsTo?:number,birthDate?:string,hireDate?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    lastName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    title;
    /** @type {?number} */
    reportsTo;
    /** @type {?string} */
    birthDate;
    /** @type {?string} */
    hireDate;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    getTypeName() { return 'CreateChinookEmployee' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateGenres {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'CreateGenres' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateInvoiceItems {
    /** @param {{invoiceId?:number,trackId?:number,unitPrice?:number,quantity?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    trackId;
    /** @type {number} */
    unitPrice;
    /** @type {number} */
    quantity;
    getTypeName() { return 'CreateInvoiceItems' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateInvoices {
    /** @param {{customerId?:number,invoiceDate?:string,billingAddress?:string,billingCity?:string,billingState?:string,billingCountry?:string,billingPostalCode?:string,total?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    customerId;
    /** @type {string} */
    invoiceDate;
    /** @type {string} */
    billingAddress;
    /** @type {string} */
    billingCity;
    /** @type {string} */
    billingState;
    /** @type {string} */
    billingCountry;
    /** @type {string} */
    billingPostalCode;
    /** @type {number} */
    total;
    getTypeName() { return 'CreateInvoices' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateMediaTypes {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'CreateMediaTypes' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreatePlaylists {
    /** @param {{name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    getTypeName() { return 'CreatePlaylists' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class CreateTracks {
    /** @param {{name?:string,albumId?:number,mediaTypeId?:number,genreId?:number,composer?:string,milliseconds?:number,bytes?:number,unitPrice?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    name;
    /** @type {?number} */
    albumId;
    /** @type {number} */
    mediaTypeId;
    /** @type {?number} */
    genreId;
    /** @type {string} */
    composer;
    /** @type {number} */
    milliseconds;
    /** @type {?number} */
    bytes;
    /** @type {number} */
    unitPrice;
    getTypeName() { return 'CreateTracks' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class DeleteAlbums {
    /** @param {{albumId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    albumId;
    getTypeName() { return 'DeleteAlbums' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteArtists {
    /** @param {{artistId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    artistId;
    getTypeName() { return 'DeleteArtists' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteChinookCustomer {
    /** @param {{customerId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    customerId;
    getTypeName() { return 'DeleteChinookCustomer' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteChinookEmployee {
    /** @param {{employeeId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    employeeId;
    getTypeName() { return 'DeleteChinookEmployee' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteGenres {
    /** @param {{genreId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    genreId;
    getTypeName() { return 'DeleteGenres' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteInvoiceItems {
    /** @param {{invoiceLineId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceLineId;
    getTypeName() { return 'DeleteInvoiceItems' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteInvoices {
    /** @param {{invoiceId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceId;
    getTypeName() { return 'DeleteInvoices' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteMediaTypes {
    /** @param {{mediaTypeId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    mediaTypeId;
    getTypeName() { return 'DeleteMediaTypes' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeletePlaylists {
    /** @param {{playlistId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    playlistId;
    getTypeName() { return 'DeletePlaylists' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class DeleteTracks {
    /** @param {{trackId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    trackId;
    getTypeName() { return 'DeleteTracks' }
    getMethod() { return 'DELETE' }
    createResponse() { return new IdResponse() }
}
export class PatchAlbums {
    /** @param {{albumId?:number,title?:string,artistId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    albumId;
    /** @type {string} */
    title;
    /** @type {number} */
    artistId;
    getTypeName() { return 'PatchAlbums' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchArtists {
    /** @param {{artistId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    artistId;
    /** @type {string} */
    name;
    getTypeName() { return 'PatchArtists' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchChinookCustomer {
    /** @param {{customerId?:number,firstName?:string,lastName?:string,company?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string,supportRepId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    customerId;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    company;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    /** @type {?number} */
    supportRepId;
    getTypeName() { return 'PatchChinookCustomer' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchChinookEmployee {
    /** @param {{employeeId?:number,lastName?:string,firstName?:string,title?:string,reportsTo?:number,birthDate?:string,hireDate?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    employeeId;
    /** @type {string} */
    lastName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    title;
    /** @type {?number} */
    reportsTo;
    /** @type {?string} */
    birthDate;
    /** @type {?string} */
    hireDate;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    getTypeName() { return 'PatchChinookEmployee' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchGenres {
    /** @param {{genreId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    genreId;
    /** @type {string} */
    name;
    getTypeName() { return 'PatchGenres' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchInvoiceItems {
    /** @param {{invoiceLineId?:number,invoiceId?:number,trackId?:number,unitPrice?:number,quantity?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceLineId;
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    trackId;
    /** @type {number} */
    unitPrice;
    /** @type {number} */
    quantity;
    getTypeName() { return 'PatchInvoiceItems' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchInvoices {
    /** @param {{invoiceId?:number,customerId?:number,invoiceDate?:string,billingAddress?:string,billingCity?:string,billingState?:string,billingCountry?:string,billingPostalCode?:string,total?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    customerId;
    /** @type {string} */
    invoiceDate;
    /** @type {string} */
    billingAddress;
    /** @type {string} */
    billingCity;
    /** @type {string} */
    billingState;
    /** @type {string} */
    billingCountry;
    /** @type {string} */
    billingPostalCode;
    /** @type {number} */
    total;
    getTypeName() { return 'PatchInvoices' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchMediaTypes {
    /** @param {{mediaTypeId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    mediaTypeId;
    /** @type {string} */
    name;
    getTypeName() { return 'PatchMediaTypes' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchPlaylists {
    /** @param {{playlistId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    playlistId;
    /** @type {string} */
    name;
    getTypeName() { return 'PatchPlaylists' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class PatchTracks {
    /** @param {{trackId?:number,name?:string,albumId?:number,mediaTypeId?:number,genreId?:number,composer?:string,milliseconds?:number,bytes?:number,unitPrice?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    trackId;
    /** @type {string} */
    name;
    /** @type {?number} */
    albumId;
    /** @type {number} */
    mediaTypeId;
    /** @type {?number} */
    genreId;
    /** @type {string} */
    composer;
    /** @type {number} */
    milliseconds;
    /** @type {?number} */
    bytes;
    /** @type {number} */
    unitPrice;
    getTypeName() { return 'PatchTracks' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class UpdateAlbums {
    /** @param {{albumId?:number,title?:string,artistId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    albumId;
    /** @type {string} */
    title;
    /** @type {number} */
    artistId;
    getTypeName() { return 'UpdateAlbums' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateArtists {
    /** @param {{artistId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    artistId;
    /** @type {string} */
    name;
    getTypeName() { return 'UpdateArtists' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateChinookCustomer {
    /** @param {{customerId?:number,firstName?:string,lastName?:string,company?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string,supportRepId?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    customerId;
    /** @type {string} */
    firstName;
    /** @type {string} */
    lastName;
    /** @type {string} */
    company;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    /** @type {?number} */
    supportRepId;
    getTypeName() { return 'UpdateChinookCustomer' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateChinookEmployee {
    /** @param {{employeeId?:number,lastName?:string,firstName?:string,title?:string,reportsTo?:number,birthDate?:string,hireDate?:string,address?:string,city?:string,state?:string,country?:string,postalCode?:string,phone?:string,fax?:string,email?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    employeeId;
    /** @type {string} */
    lastName;
    /** @type {string} */
    firstName;
    /** @type {string} */
    title;
    /** @type {?number} */
    reportsTo;
    /** @type {?string} */
    birthDate;
    /** @type {?string} */
    hireDate;
    /** @type {string} */
    address;
    /** @type {string} */
    city;
    /** @type {string} */
    state;
    /** @type {string} */
    country;
    /** @type {string} */
    postalCode;
    /** @type {string} */
    phone;
    /** @type {string} */
    fax;
    /** @type {string} */
    email;
    getTypeName() { return 'UpdateChinookEmployee' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateGenres {
    /** @param {{genreId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    genreId;
    /** @type {string} */
    name;
    getTypeName() { return 'UpdateGenres' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateInvoiceItems {
    /** @param {{invoiceLineId?:number,invoiceId?:number,trackId?:number,unitPrice?:number,quantity?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceLineId;
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    trackId;
    /** @type {number} */
    unitPrice;
    /** @type {number} */
    quantity;
    getTypeName() { return 'UpdateInvoiceItems' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateInvoices {
    /** @param {{invoiceId?:number,customerId?:number,invoiceDate?:string,billingAddress?:string,billingCity?:string,billingState?:string,billingCountry?:string,billingPostalCode?:string,total?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    invoiceId;
    /** @type {number} */
    customerId;
    /** @type {string} */
    invoiceDate;
    /** @type {string} */
    billingAddress;
    /** @type {string} */
    billingCity;
    /** @type {string} */
    billingState;
    /** @type {string} */
    billingCountry;
    /** @type {string} */
    billingPostalCode;
    /** @type {number} */
    total;
    getTypeName() { return 'UpdateInvoices' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateMediaTypes {
    /** @param {{mediaTypeId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    mediaTypeId;
    /** @type {string} */
    name;
    getTypeName() { return 'UpdateMediaTypes' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdatePlaylists {
    /** @param {{playlistId?:number,name?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    playlistId;
    /** @type {string} */
    name;
    getTypeName() { return 'UpdatePlaylists' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class UpdateTracks {
    /** @param {{trackId?:number,name?:string,albumId?:number,mediaTypeId?:number,genreId?:number,composer?:string,milliseconds?:number,bytes?:number,unitPrice?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    trackId;
    /** @type {string} */
    name;
    /** @type {?number} */
    albumId;
    /** @type {number} */
    mediaTypeId;
    /** @type {?number} */
    genreId;
    /** @type {string} */
    composer;
    /** @type {number} */
    milliseconds;
    /** @type {?number} */
    bytes;
    /** @type {number} */
    unitPrice;
    getTypeName() { return 'UpdateTracks' }
    getMethod() { return 'PUT' }
    createResponse() { return new IdResponse() }
}
export class CreateBooking {
    /** @param {{name?:string,photo?:string,roomType?:RoomType,roomNumber?:number,cost?:number,bookingStartDate?:string,bookingEndDate?:string,notes?:string,couponId?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /**
     * @type {string}
     * @description Name this Booking is for */
    name;
    /** @type {?string} */
    photo;
    /** @type {RoomType} */
    roomType;
    /** @type {number} */
    roomNumber;
    /** @type {number} */
    cost;
    /** @type {string} */
    bookingStartDate;
    /** @type {?string} */
    bookingEndDate;
    /** @type {?string} */
    notes;
    /** @type {?string} */
    couponId;
    getTypeName() { return 'CreateBooking' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class UpdateBooking {
    /** @param {{id?:number,name?:string,roomType?:RoomType,roomNumber?:number,cost?:number,bookingStartDate?:string,bookingEndDate?:string,notes?:string,couponId?:string,cancelled?:boolean}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {?string} */
    name;
    /** @type {?RoomType} */
    roomType;
    /** @type {?number} */
    roomNumber;
    /** @type {?number} */
    cost;
    /** @type {?string} */
    bookingStartDate;
    /** @type {?string} */
    bookingEndDate;
    /** @type {?string} */
    notes;
    /** @type {?string} */
    couponId;
    /** @type {?boolean} */
    cancelled;
    getTypeName() { return 'UpdateBooking' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class DeleteBooking {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeleteBooking' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreateCoupon {
    /** @param {{description?:string,discount?:number,expiryDate?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    description;
    /** @type {number} */
    discount;
    /** @type {string} */
    expiryDate;
    getTypeName() { return 'CreateCoupon' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class UpdateCoupon {
    /** @param {{id?:string,description?:string,discount?:number,expiryDate?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    id;
    /** @type {string} */
    description;
    /** @type {number} */
    discount;
    /** @type {string} */
    expiryDate;
    getTypeName() { return 'UpdateCoupon' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class DeleteCoupon {
    /** @param {{id?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    id;
    getTypeName() { return 'DeleteCoupon' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
export class CreateFileSystemItem {
    /** @param {{fileAccessType?:FileAccessType,file?:FileSystemFile}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {?FileAccessType} */
    fileAccessType;
    /** @type {FileSystemFile} */
    file;
    getTypeName() { return 'CreateFileSystemItem' }
    getMethod() { return 'POST' }
    createResponse() { return new FileSystemItem() }
}
export class CreatePlayer {
    /** @param {{firstName?:string,lastName?:string,email?:string,phoneNumbers?:Phone[],profileId?:number,savedLevelId?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {string} */
    firstName;
    /** @type {?string} */
    lastName;
    /** @type {?string} */
    email;
    /** @type {?Phone[]} */
    phoneNumbers;
    /** @type {number} */
    profileId;
    /** @type {?string} */
    savedLevelId;
    getTypeName() { return 'CreatePlayer' }
    getMethod() { return 'POST' }
    createResponse() { return new IdResponse() }
}
export class UpdatePlayer {
    /** @param {{id?:number,firstName?:string,lastName?:string,email?:string,phoneNumbers?:Phone[],profileId?:number,savedLevelId?:string}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    /** @type {string} */
    firstName;
    /** @type {?string} */
    lastName;
    /** @type {?string} */
    email;
    /** @type {?Phone[]} */
    phoneNumbers;
    /** @type {?number} */
    profileId;
    /** @type {?string} */
    savedLevelId;
    getTypeName() { return 'UpdatePlayer' }
    getMethod() { return 'PATCH' }
    createResponse() { return new IdResponse() }
}
export class DeletePlayer {
    /** @param {{id?:number}} [init] */
    constructor(init) { Object.assign(this, init) }
    /** @type {number} */
    id;
    getTypeName() { return 'DeletePlayer' }
    getMethod() { return 'DELETE' }
    createResponse() { }
}
