--USE master;
--GO

--IF DB_ID('BillGates') IS NOT NULL
--BEGIN
--    ALTER DATABASE BillGates SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--    DROP DATABASE BillGates;
--END
--GO

--CREATE DATABASE BillGates;
--GO

--USE BillGates;
--GO

--CREATE TABLE clientes (
--    num INT IDENTITY PRIMARY KEY,
--    nome NVARCHAR(50) NOT NULL UNIQUE,
--    categoria NVARCHAR(50) CHECK (categoria IN ('Alfa', 'Bravo', 'Charlie')),
--    datanasc DATE,
--    idade AS DATEDIFF(YEAR, datanasc, GETDATE()),
--    tutor INT,
--    CONSTRAINT fk_clientes_clientes
--        FOREIGN KEY (tutor) REFERENCES clientes(num)
--);
--GO

--INSERT INTO clientes (nome, categoria, datanasc)
--VALUES
--('Tio Patinhas', 'Alfa', '1943-01-01'),
--('Pato Donald', 'Alfa', '1983-01-01'),
--('Margarida', 'Charlie', '1972-01-01'),
--('Pateta', 'Bravo', '1971-01-01');
--GO

--UPDATE clientes
--SET tutor =
--    CASE num
--        WHEN 1 THEN NULL
--        WHEN 2 THEN 1
--        ELSE 2
--    END;
--GO

--CREATE TABLE carros (
--    idcar INT IDENTITY PRIMARY KEY,
--    modelo NVARCHAR(50) NOT NULL,
--    phora DECIMAL(10,2) CHECK (phora BETWEEN 0 AND 200)
--);
--GO

--SET IDENTITY_INSERT carros ON;

--INSERT INTO carros (idcar, modelo, phora)
--VALUES
--(10, 'Ford Fiesta', 15.50),
--(11, 'Mercedes CLA 200', 20.50),
--(12, 'Fiat 600', 10.50),
--(13, 'Ferrari F40', 120.50);

--SET IDENTITY_INSERT carros OFF;
--GO

--CREATE TABLE alugueres (
--    idal INT IDENTITY PRIMARY KEY,
--    num INT FOREIGN KEY REFERENCES clientes(num),
--    idcar INT FOREIGN KEY REFERENCES carros(idcar),
--    inicio DATETIME DEFAULT GETDATE(),
--    fim DATETIME,
--    tempo AS CAST((DATEDIFF(MINUTE, inicio, fim) / 60.0) AS FLOAT),
--    custo DECIMAL(10,2),
--    CONSTRAINT chk_tempo CHECK (DATEDIFF(MINUTE, inicio, fim) >= 0)
--);
--GO

--CREATE VIEW VW_Sorte AS
--SELECT RAND() AS sorte;
--GO

--CREATE FUNCTION fn_sorte (@min INT, @max INT)
--RETURNS INT
--AS
--BEGIN
--    DECLARE @delta INT = @max - @min + 1;

--    RETURN FLOOR((SELECT sorte FROM VW_Sorte) * @delta) + @min;
--END;
--GO

--INSERT INTO alugueres (num, idcar, inicio, fim)
--SELECT
--    c.num,
--    ca.idcar,
--    GETDATE(),
--    DATEADD(MINUTE, dbo.fn_sorte(30, 3600), GETDATE())
--FROM clientes c
--CROSS JOIN carros ca;
--GO

--UPDATE a
--SET custo = ca.phora * a.tempo
--FROM alugueres a
--INNER JOIN carros ca ON ca.idcar = a.idcar;
--GO

--SELECT * FROM clientes;
--SELECT * FROM carros;
--SELECT * FROM alugueres;
--GO

--SELECT *
--FROM (
--    SELECT *,
--           ROW_NUMBER() OVER (ORDER BY idade DESC) AS ordem
--    FROM clientes
--) t
--WHERE t.ordem BETWEEN 2 AND 3;
--GO

--WITH cte AS (
--    SELECT *,
--           ROW_NUMBER() OVER (ORDER BY idade DESC) AS ordem
--    FROM clientes
--)
--SELECT *
--FROM cte
--WHERE ordem IN (1, 3);
--GO

--SELECT *
--FROM clientes
--ORDER BY idade DESC
--OFFSET 3 ROWS FETCH NEXT 3 ROWS ONLY;
--GO

DROP VIEW IF EXISTS vw_a;
GO

CREATE VIEW vw_a
AS
SELECT *
FROM clientes
WHERE num <= 3;
GO

DROP VIEW IF EXISTS vw_b;
GO

CREATE VIEW vw_b
AS
SELECT *
FROM clientes
WHERE num >= 3;
GO


SELECT *
FROM vw_a;

SELECT *
FROM vw_b;


SELECT *
FROM vw_a
EXCEPT
SELECT *
FROM vw_b;


WITH cte AS (
    SELECT 
        num,
        nome,
        categoria,
        datanasc,
        idade,
        tutor,
        10 AS Nivel
    FROM clientes
    WHERE tutor IS NULL

    UNION ALL

    SELECT 
        c.num,
        c.nome,
        c.categoria,
        c.datanasc,
        c.idade,
        c.tutor,
        cte.Nivel - 1 AS Nivel
    FROM cte
    INNER JOIN clientes c 
        ON c.tutor = cte.num
)
SELECT *
FROM cte
OPTION (MAXRECURSION 100);

delete from alugueres where num = 3;
delete from alugueres where num = 4 and idcar < 12;

select * from clientes c where not exists(
    select idcar from carros
    except
    select idcar from alugueres where num = c.num
)

select * from alugueres;

update alugueres set custo = ca.phora * a.tempo 
from carros ca join alugueres a on a.idcar = ca.idcar;

select num, idcar, sum(custo) soma, avg(custo) media, count(*) linhas
from alugueres group by cube (num, idcar)
having count(*) > 3;

select num, idcar, sum(custo) soma, avg(custo) media, count(*) linhas
from alugueres group by cube (num, idcar)
having idcar is null;

select * from alugueres;
insert into alugueres (num, idcar, inicio, fim)
values (1, 11, getdate(), dateadd(minute, 300, getdate()));

go
 create trigger trg_aluguer on alugueres
 after insert, update
 as
 begin
  update alugueres set custo = i.tempo * ca.phora from alugueres a join carros ca
         on ca.idcar = a.idcar
         join inserted i on i.idal = a.idal
 end
go


-- procedure (soma do custo)
go
 create proc sp_um(@num int = null, @soma decimal(10,2) out)
 as
 begin
  declare @linhas int;
  select * from alugueres where num = @num or @num is null;
  set @linhas = @@rowcount; -- @@rowcount -> variavel global do sql server que retorna o numero de linhas afetadas pela ultima query
  select @soma = sum(custo) from alugueres group by num having num = @num or @num is null;
 end
go

declare @s decimal(10,2), @c int;
exec @c = sp_um null, @soma = @s out;

select @c as registos, @s as SomaCustos;