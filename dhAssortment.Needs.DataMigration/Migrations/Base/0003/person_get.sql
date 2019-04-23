CREATE OR REPLACE FUNCTION {{schema_name}}.person_get()
  RETURNS SETOF {{schema_name}}.person AS
$func$
BEGIN
   RETURN QUERY
   TABLE {{schema_name}}.person;  -- shorthand for: SELECT * FROM public.person
END
$func$  LANGUAGE plpgsql;
ALTER FUNCTION {{schema_name}}.person_get()    OWNER TO postgres;