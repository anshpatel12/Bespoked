create view public.vw_TotalSales as
 SELECT s."SaleId",
    p."Name" AS productname,
    (c."FirstName"::text || ' '::text) || c."LastName"::text AS customername,
    s."SaleDate",
    s."QuantitySold",
    p."SalePrice" * s."QuantitySold"::double precision AS totalsaleprice,
    (sp."FirstName"::text || ' '::text) || sp."LastName"::text AS salespersonname,
    p."SalePrice" * p."CommissionPct" / 100::double precision * s."QuantitySold"::double precision AS salespersoncommission
   FROM "Sales" s
     JOIN "Products" p ON s."ProductId" = p."ProductId"
     JOIN "Customer" c ON s."CustomerId" = c."CustomerId"
     JOIN "Salesperson" sp ON s."SalespersonId" = sp."SalespersonId";