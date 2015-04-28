
create table Vuosimalli
(
	Id int identity(1, 1) not null,
	Vuosi integer not null,
	
	constraint pk_Vuosimalli primary key (Id),
);

create table Ajoneuvotyyppi
(
	Id int identity(1,1) not null,
	Tyyppi nvarchar(50) not null,
	
	constraint pk_Ajoneuvotyyppi primary key (Id),
);

create table Vari
(
	Id int identity(1,1) not null,
	Vari nvarchar(255) not null,
	
	constraint pk_Vari primary key (Id),	
);

create table Ajoneuvo
(
	Id int identity(1, 1) not null,
	Reknro nvarchar(7) not null,
	Merkki nvarchar(50) not null,
	Malli nvarchar(50) not null,
	Tyyppi integer not null,
	Vuosimalli integer not null,
	Vari integer  null,
	Rekisterissa bit not null,
	
	constraint pk_Auto primary key (Id),
    constraint fk_Ajoneuvo_Vuosimalli foreign key (Vuosimalli) references Vuosimalli(Id),
	constraint fk_Ajoneuvo_Vari foreign key (Vari) references Vari(Id),
	constraint fk_Ajoneuvo_AjoneuvoTyyppi foreign key (tyyppi) references AjoneuvoTyyppi(Id)

);

