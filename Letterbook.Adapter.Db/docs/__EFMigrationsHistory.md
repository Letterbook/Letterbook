classDiagram
direction BT
class ActorAudience {
   text AudiencesId
   text MembersId
}
class Actors {
   text Host
   text LocalId
   text Id
}
class Audiences {
   text Id
}
class JoinObjectActor {
   text ObjectId
   integer Relationship
   text ActorId
   text ApObject3Id
}
class JoinObjectAudience {
   text AudienceId
   text ObjectsId
}
class Objects {
   integer Type
   text LocalId
   text Host
   text ActorId
   text Id
}
class __EFMigrationsHistory {
   varchar(32) ProductVersion
   varchar(150) MigrationId
}

ActorAudience  -->  Actors : MembersId:Id
ActorAudience  -->  Audiences : AudiencesId:Id
JoinObjectActor  -->  Actors : ActorId:Id
JoinObjectActor  -->  Objects : ApObject3Id:Id
JoinObjectAudience  -->  Audiences : AudienceId:Id
JoinObjectAudience  -->  Objects : ObjectsId:Id
Objects  -->  Actors : ActorId:Id
