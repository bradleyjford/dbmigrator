create table [Person]
(
    [Id] uniqueidentifier not null,
    [Name] nvarchar(100) not null,
    [BirthDate] date null,
    constraint [PK_Person] primary key (Id) clustered
)

GO

alter table [Person] drop column BirthDate
