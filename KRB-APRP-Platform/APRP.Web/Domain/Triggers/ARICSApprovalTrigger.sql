-----aricsapproval_trigger---------------------------------------------------

--Insert Trigger
CREATE OR REPLACE FUNCTION aricsapproval_insert() RETURNS trigger AS
$$
  BEGIN
		--Dealing with the history table
		INSERT INTO public."ARICSApprovalh"
		  (
			username ,
			comment,
			aricsmasterapprovalid,
			year,
			updatedby,
			updateddate,
			createdby,
			creationdate,
			beginlifespan,
			endlifespan,
			aricsapprovalid,
			aricsapprovallevelid
		)
		VALUES
		  (
			NEW.username ,
			NEW.comment,
			NEW.aricsmasterapprovalid,
			NEW.year,
			NEW.updatedby,
			NEW.updateddate,
			NEW.createdby,
			NEW.creationdate,
			current_timestamp,
			NEW.endlifespan,
			NEW.id,
			NEW.aricsapprovallevelid

		);
		--End of Dealing with history table

    RETURN NEW;
  END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER aricsapproval_insert_trigger
AFTER INSERT ON public."ARICSApproval"
  FOR EACH ROW EXECUTE PROCEDURE aricsapproval_insert();


----Delete Trigger
CREATE OR REPLACE FUNCTION aricsapproval_delete() RETURNS trigger AS
$$
  BEGIN
 --Begin Dealing with the history table
    UPDATE public."ARICSApprovalh"
      SET endlifespan = current_timestamp
        WHERE aricsapprovalid= OLD.id and endlifespan IS NULL;
 --End of Dealing with the history table
    RETURN NULL;
  END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER aricsapproval_delete_trigger
AFTER DELETE ON public."ARICSApproval"
  FOR EACH ROW EXECUTE PROCEDURE aricsapproval_delete();

--Update Trigger

CREATE OR REPLACE FUNCTION public.aricsapproval_update()
  RETURNS trigger AS
$BODY$
  BEGIN
 --Begin Dealing with the history table
    UPDATE public."ARICSApprovalh"
      SET endlifespan = current_timestamp
      WHERE endlifespan IS NULL and aricsapprovalid = OLD.id;

		--Dealing with the history table
		INSERT INTO public."ARICSApprovalh"
		  (
			username ,
			comment,
			aricsmasterapprovalid,
			year,
			updatedby,
			updateddate,
			createdby,
			creationdate,
			beginlifespan,
			endlifespan,
			aricsapprovalid,
			aricsapprovallevelid
		)
		VALUES
		  (
			NEW.username ,
			NEW.comment,
			NEW.aricsmasterapprovalid,
			NEW.year,
			NEW.updatedby,
			NEW.updateddate,
			NEW.createdby,
			NEW.creationdate,
			current_timestamp,
			NEW.endlifespan,
			NEW.id,
			NEW.aricsapprovallevelid

		);
		--End of Dealing with history table

    RETURN NEW;
 --End of Dealing with the history table

  END;
$BODY$
  LANGUAGE plpgsql;

CREATE TRIGGER aricsapproval_update_trigger
AFTER UPDATE ON public."ARICSApproval"
  FOR EACH ROW EXECUTE PROCEDURE aricsapproval_update();
