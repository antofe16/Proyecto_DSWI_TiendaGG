if  DB_ID('TiendaGG_Web') is not null
use master
drop database TiendaGG_Web
go


create database TiendaGG_Web
go

use TiendaGG_Web
go


create table tb_Administrador(
cod_adm		varchar(10) constraint PK_ADM primary key not null,
nom_adm		varchar(50)		not null,
ape_adm		varchar(50)		not null,		
dir_adm		varchar(60)		not null,
email_adm	varchar(100)	not null,
tel_adm		varchar(15)		not null,
usu_adm		varchar(20)		not null,
cla_adm		varchar(15)		not null,
est_adm		int		not null	default 1
)
go

insert tb_administrador 
values('ADM001','Antonio','Felix Cuya',
'Av. Victor Castro Iglesias 1184','antonio_felixcuya@hotmail.com',
960865694,'aafelix','melan',default)
go

create table tb_Cliente(
cod_cli		varchar(10) constraint PK_CLI primary key not null,
nom_cli		varchar(50)		not null,
ape_cli		varchar(50)		not null,
email_cli	varchar(100)	not null,
dni_cli		char(8)		constraint UNQ_DNI_CLI unique not null,
telf_cli	varchar(15)		not null,
dir_cli		varchar(60)		not null,
usu_cli 	varchar(20)		not null,
cla_cli		varchar(15)		not null,
est_cli		int	not null	default 1
)
go

/*insert into tb_Cliente values('CLI001','Ramon','Castilla','ramoncito@gmail.com','10023543',987654321,'Av. El sol 123','ramcast10','yope01',default)
go
insert into tb_Cliente values('CLI002','Gloria','Ramos Castriel','glori@gmail.com','20032345',987654321,'Av. El Carmen 123','glori20','saturno',default)
go*/

create proc usp_create_cliente
@nom varchar(50),
@ape varchar(50),
@email varchar(100),
@dni char(8),
@telf varchar(15),
@dir varchar(60),
@usu varchar(20),
@cla varchar(15)
as
	declare @cod varchar(10), @num int, @aux varchar(5)
		if not exists (select * from tb_Cliente) 
			begin
				set @cod = 'CLI0001'
			end
		else
			begin
				select @cod = max(cod_cli) from tb_Cliente
				set @num = CAST(RIGHT(@cod,4) as int)+1
				set @aux = Cast(@num as varchar(4))
				set @cod = 'CLI'+REPLICATE('0',4-LEN(@aux))+@aux
			end
	begin
		insert into tb_Cliente values(@cod,@nom,@ape,@email,@dni,@telf,@dir, @usu,@cla,default)
	end
go

usp_create_cliente 'Ramon','Castilla','ramoncito@gmail.com','10023542',987654321,'Av. El sol 123','ramcast10','yope01'
go

select * from tb_Cliente

create table tb_Categoria(
cod_cat		char(3) constraint PK_CAT primary key	not null,
nom_cat 	varchar(30)	not null
)
go
insert tb_Categoria values ('C01','Videojuegos'),
						('C02','Juguetes'),
						('C03','Ropa'),
                        ('C04','Accesorios'),
						('C05','Consolas')
go

create table tb_Producto(
cod_pro		varchar(10)	not null constraint PK_PRO primary key,
desc_pro	varchar(50)	not null,
pre_pro		decimal(8,2)not null,
stock_pro	int			not null,
cod_cat		char(3)	not null constraint FK_CAT_PRO references tb_Categoria,
est_pro		int default 1,
img_pro		nvarchar(100) not null
)
go
/* Estado
	1 -> Activo
    0 -> Inactivo
*/

-- videojuegos
-- aventura
insert into tb_Producto values('P0001','Minecraft',50.0,100,'C01',default,'~/img/prod/P0001.jpg')
insert into tb_Producto values('P0002','Middle-earth: Shadow of War',32.49,100,'C01',default,'~/img/prod/P0002.jpg')
insert into tb_Producto values('P0003','Terraria',13.97,100,'C01',default,'~/img/prod/P0003.jpg')
-- terror

insert into tb_Producto values('P0004','Outlast',15.50,100,'C01',default,'~/img/prod/P0004.jpg')
insert into tb_Producto values('P0005','Resident Evil 2',29.60,100,'C01',default,'~/img/prod/P0005.jpg')
go



create proc usp_insert_producto
@desc varchar(50),
@pre decimal,
@stk int,
@idcat char(3),
@img varchar(100)
as
	declare @cod varchar(10), @num Int, @aux varchar(4)
		if not exists (select * from tb_Producto) 
			begin
				set @cod = 'P0001'
			end
		else
			begin
				select @cod = max(cod_pro) from tb_Producto
				set @num = CAST(RIGHT(@cod,4) as int)+1
				set @aux = Cast(@num as varchar(4))
				set @cod = 'P'+REPLICATE('0',4-LEN(@aux))+@aux
			end
	begin
		insert into tb_Producto values(@cod,@desc,@pre, @stk,@idcat,default,@img)
	end
go

usp_insert_producto 'Alien Isolation',47.47,100,'C01','~/img/prod/P0006.jpg'
go


create proc usp_update_producto
@cod varchar(10),
@desc varchar(50),
@pre decimal,
@stk int,
@idcat char(3),
@img varchar(100)
as
	begin
		update tb_Producto set desc_pro = @desc, pre_pro = @pre, stock_pro = @stk, cod_cat = @idcat, img_pro = @img
		where cod_pro = @cod
	end
go

usp_update_producto 'P0006','Alien Isolation',45.00,100,'C01','~/img/prod/P0006.jpg'
go

create proc usp_delete_producto
@cod varchar(10)
as
	begin
		update tb_Producto set est_pro = 0, stock_pro = 0
		where cod_pro = @cod
	end
go

/*usp_delete_producto 'P0006'
go*/

-- accion
insert into tb_Producto values('P0007','DOOM',60.90,100,'C01',default,'~/img/prod/P0007.jpg')
insert into tb_Producto values('P0008','STAR WARS Jedi: Fallen Order',199.00,100,'C01',default,'~/img/prod/P0008.jpg')
insert into tb_Producto values('P0009','Terminator: Resistance',100.00,100,'C01',default,'~/img/prod/P0009.jpg')
-- estrategia
insert into tb_Producto values('P0010','StarCraft II',19.99,100,'C01',default,'~/img/prod/P0010.jpg')
insert into tb_Producto values('P0011','WarCraft III',25.50,100,'C01',default,'~/img/prod/P0011.jpg')
insert into tb_Producto values('P0012','Age of Empires III',67.50,100,'C01',default,'~/img/prod/P0012.jpg')
-- RPG
insert into tb_Producto values('P0013','Final Fantasy xv',20.0,100,'C01',default,'~/img/prod/P0013.jpg')
insert into tb_Producto values('P0014','DRAGON QUEST XI: Echoes of an Elusive Age',159.99,100,'C01',default,'~/img/prod/P0014.jpg')
insert into tb_Producto values('P0015','FINAL FANTASY X/X-2 HD Remaster',73.95,100,'C01',default,'~/img/prod/P0015.jpg')
-- Disparos...
insert into tb_Producto values('P0016','Destiny 2',36.60,100,'C01',default,'~/img/prod/P0016.jpg')
insert into tb_Producto values('P0017','Gears of war 3',59.35,100,'C01',default,'~/img/prod/P0017.jpg')
insert into tb_Producto values('P0018','Call Of Duty Black OPS III',87.90,100,'C01',default,'~/img/prod/P0018.jpg')
--
-- Ropa
insert into tb_Producto values('P0019','Polo Clash Royals',39.90,100,'C03',default,'~/img/prod/P0019.jpg')
insert into tb_Producto values('P0020','Sudadera Zelda',20.99,100,'C03',default,'~/img/prod/P0020.jpg')
insert into tb_Producto values('P0021','Polo Player',17.99,100,'C03',default,'~/img/prod/P0021.jpg')
insert into tb_Producto values('P0022','Polera y short Mario Bros',17.99,100,'C03',default,'~/img/prod/P0022.jpg')
insert into tb_Producto values('P0023','Short Navier',16.99,100,'C03',default,'~/img/prod/P0023.jpg')
insert into tb_Producto values('P0024','Casaca Fornite',15.90,100,'C03',default,'~/img/prod/P0024.jpg')
-- juguete
insert into tb_Producto values('P0025','Funko Kratos',16.99,100,'C02',default,'~/img/prod/P0025.jpg')
insert into tb_Producto values('P0026','Funko Mega Man',13.99,100,'C02',default,'~/img/prod/P0026.jpg')
insert into tb_Producto values('P0027','Funko Pudge',20.90,100,'C02',default,'~/img/prod/P0027.jpg')
insert into tb_Producto values('P0028','Funko Nathan Drake',10.99,100,'C02',default,'~/img/prod/P0028.jpg')
insert into tb_Producto values('P0029','Funko Sparkle Specialist',19.99,100,'C02',default,'~/img/prod/P0029.jpg')
insert into tb_Producto values('P0030','Funko Robotic Santiage',50.99,100,'C02',default,'~/img/prod/P0030.jpg')
-- accesorio
insert into tb_Producto values('P0031','Ps4 Joystick - Estampado guerra',33.99,100,'C04',default,'~/img/prod/P0031.jpg')
insert into tb_Producto values('P0032','Nintendo Labo',49.99,100,'C04',default,'~/img/prod/P0032.jpg')
insert into tb_Producto values('P0033','Audifonos Nike azul',49.99,100,'C04',default,'~/img/prod/P0033.jpg')
insert into tb_Producto values('P0034','Audifonos Halion naranja',30.99,100,'C04',default,'~/img/prod/P0034.jpg')
insert into tb_Producto values('P0035','Taza Pacman',4.99,100,'C04',default,'~/img/prod/P0035.jpg')
insert into tb_Producto values('P0036','Taza la vida',3.99,100,'C04',default,'~/img/prod/P0036.jpg')
--consola
insert into tb_Producto values('P0037','Play Station 4',33.99,100,'C05',default,'~/img/prod/P0037.jpg')
insert into tb_Producto values('P0038','Play Station 5',33.99,100,'C05',default,'~/img/prod/P0038.jpg')
insert into tb_Producto values('P0039','Xbox One',33.99,100,'C05',default,'~/img/prod/P0039.jpg')
insert into tb_Producto values('P0040','Nintendo Switch',33.99,100,'C05',default,'~/img/prod/P0040.jpg')
insert into tb_Producto values('P0041','Nintendo Wii',33.99,100,'C05',default,'~/img/prod/P0041.jpg')
go

create or alter proc usp_consulta_multiple
@p1 decimal,
@p2 decimal,
@term varchar(50),
@idcat varchar(3)
as
begin
	if @term is null and @idcat != 'C00' and @p1 > 0 and @p2 > 0
	select * from tb_Producto
	where cod_cat = @idcat and pre_pro between @p1 and @p2

	else if @term is not null and @idcat = 'C00' and @p1 <= 0 and @p2 <= 0
	select * from tb_Producto
	where desc_pro like '%'+@term+'%'

	else if @term is null and @idcat = 'C00' and @p1 > 0 and @p2 > 0
	select * from tb_Producto
	where desc_pro like '%'+@term+'%' and pre_pro between @p1 and @p2

	else if @term is null and @idcat != 'C00' and @p1 <= 0 and @p2 <= 0
	select * from tb_Producto
	where cod_cat = @idcat

	else if @term is not null and @idcat != 'C00' and @p1 <= 0 and @p2 <= 0
	select * from tb_Producto
	where desc_pro like '%'+@term+'%' and cod_cat = @idcat

	else if @term is not null and @idcat = 'C00' and @p1 > 0 and @p2 > 0
	select * from tb_Producto
	where desc_pro like '%'+@term+'%' and pre_pro between @p1 and @p2

	else if @term is not null and @idcat != 'C00' and @p1 > 0 and @p2 > 0
	select * from tb_Producto
	where desc_pro like '%'+@term+'%' and pre_pro between @p1 and @p2 and cod_cat = @idcat
	
	else
	select * from tb_Producto

end
go

usp_consulta_multiple 0,0,'','C00'
go

select * from tb_Producto
go


create proc usp_login_cliente
@user varchar(100),
@pass varchar(15)
as
begin
	select * from tb_Cliente where usu_cli = @user and cla_cli = @pass
end
go

usp_login_cliente 'ramcast10','yope01'
go

create proc usp_login_admin
@user varchar(100),
@pass varchar(15)
as
begin
	select * from tb_Administrador where usu_adm = @user and cla_adm = @pass
end
go

usp_login_admin 'aafelix','melan'
go



create table tb_CabBoleta(
cod_bol		varchar(10)	not null constraint PK_BOL_CAB primary key,
fec_bol		date	not null,
cod_cli		varchar(10)	not null constraint FK_CLI_BOL references tb_Cliente,
tot_bol	decimal(8,2)	not null
)
go

create function fx_AutogenerarBoleta()
returns varchar(10)
as
begin
	declare @cod varchar(10), @num int, @aux varchar(5)
		if not exists (select * from tb_CabBoleta) 
			begin
				set @cod = 'B0001'
			end

		else
			begin
				select @cod = max(cod_bol) from tb_CabBoleta
				set @num = CAST(RIGHT(@cod,4) as int)+1
				set @aux = Cast(@num as varchar(4))
				set @cod = 'B'+REPLICATE('0',4-LEN(@aux))+@aux
			end
		return @cod
end
go
		

select dbo.fx_AutogenerarBoleta()
go

create table tb_DetBoleta(
cod_bol		varchar(10)	not null constraint FK_CAB_DET references tb_CabBoleta,
cod_pro		varchar(10)	not null constraint FK_PRO_DET references tb_Producto,
des_pro		varchar(50) not null,
can_pro		int	not null,
pre_pro	decimal(8,2) not null,
constraint PK_BOL_DET primary key(cod_bol,cod_pro)
)
go

create table tb_Reporte(
cod_rep int constraint PK_REP primary key identity(1,1),
fec_rep date
)
go

select * from tb_Producto
select * from tb_Categoria
select *  from tb_CabBoleta



/*Select para reporte de Ventas por Mes*/

select Month(fec_bol),Year(fec_bol) from tb_CabBoleta
group by fec_bol

select Month(fec_bol) 'Mes', count(cod_bol) 'Ventas realizadas', sum(tot_bol) 'Total acumulado' from tb_CabBoleta
where Month(fec_bol) =7 and Month(fec_bol) =2020
group by fec_bol

select Month(fec_bol),des_pro ,sum(can_pro)  from tb_DetBoleta d
join tb_CabBoleta c on d.cod_bol=c.cod_bol
where Month(fec_bol) =7 and Year(fec_bol) =2020
group by fec_bol,cod_pro, des_pro


