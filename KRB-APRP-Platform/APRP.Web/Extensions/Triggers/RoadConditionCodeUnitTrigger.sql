-----roadconditioncodeunith_trigger---------------------------------------------------


--Insert Trigger
CREATE OR REPLACE FUNCTION roadconditioncodeunit_insert
() RETURNS trigger AS
$$
BEGIN
	INSERT INTO "public"."RoadConditionCodeUnitAudit"
		(
		"RoadCondition",
		"Rate",
		"ActivitiesRequired",
		"SurfaceTypeId",
		"RoadConditionCodeUnitId",
		"CreationDate",
		"CreatedBy"
		)
	VALUES
		(
			NEW."RoadCondition",
			NEW."Rate",
			NEW."ActivitiesRequired",
			NEW."SurfaceTypeId",
			NEW."ID",
			current_timestamp,
			inet_client_addr()
        );
	--Dealing with the history table
	INSERT INTO "public"."RoadConditionCodeUnith"
		(
		"RoadCondition",
		"Rate",
		"ActivitiesRequired",
		"SurfaceTypeId",
		"BeginLifeSpan",
		"RoadConditionCodeUnitId"
		)
	VALUES
		(

			NEW."RoadCondition",
			NEW."Rate",
			NEW."ActivitiesRequired",
			NEW."SurfaceTypeId",
			current_timestamp,
			NEW."ID"

	);
	--End of Dealing with history table

	RETURN NEW;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER roadconditioncodeunit_insert_trigger
AFTER
INSERT ON "public"."RoadConditionCodeUnit"
FOR
EACH
ROW
EXECUTE PROCEDURE roadconditioncodeunit_insert
();


----Delete Trigger
CREATE OR REPLACE FUNCTION roadconditioncodeunit_delete
() RETURNS trigger AS
$$
BEGIN
	UPDATE "public"."RoadConditionCodeUnitAudit"
      SET deleted = current_timestamp, deleted_by = inet_client_addr()
        WHERE deleted IS NULL and "RoadConditionCodeUnitId" = OLD."ID";
	--Begin Dealing with the history table
	UPDATE "public"."RoadConditionCodeUnith"
      SET "EndLifeSpan" = current_timestamp
        WHERE "RoadConditionCodeUnitId"= OLD."ID" and EndLifeSpan IS NULL;
	--End of Dealing with the history table
	RETURN NULL;
END;
$$
LANGUAGE plpgsql;

CREATE TRIGGER roadconditioncodeunit_delete_trigger
AFTER
DELETE ON "public"."RoadConditionCodeUnit"
  FOR EACH
ROW
EXECUTE PROCEDURE roadconditioncodeunit_delete
();

--Update Trigger

CREATE OR REPLACE FUNCTION "public"."roadconditioncodeunit_update"
()
  RETURNS trigger AS
$BODY$
BEGIN
	UPDATE "public"."RoadConditionCodeUnitAudit"
      SET deleted = current_timestamp, deleted_by = inet_client_addr()
      WHERE deleted IS NULL and "RoadConditionCodeUnitId" = OLD."ID";

	INSERT INTO "public"."RoadConditionCodeUnitAudit"
		(
		"RoadCondition",
		"Rate",
		"ActivitiesRequired",
		"SurfaceTypeId",
		"RoadConditionCodeUnitId",
		"CreationDate",
		"CreatedBy"
		)
	VALUES
		(
			NEW."RoadCondition",
			NEW."Rate",
			NEW."ActivitiesRequired",
			NEW."SurfaceTypeId",
			NEW."ID",
			current_timestamp,
			inet_client_addr()
        );

	--Begin Dealing with the history table
	UPDATE "public"."RoadConditionCodeUnith"
      SET "EndLifeSpan" = current_timestamp
      WHERE "EndLifeSpan" IS NULL and "RoadConditionCodeUnitId" = OLD."ID";

	INSERT INTO "public"."RoadConditionCodeUnith"
		(
		"RoadCondition",
		"Rate",
		"ActivitiesRequired",
		"SurfaceTypeId",
		"BeginLifeSpan",
		"RoadConditionCodeUnitId"
		)
	VALUES
		(

			NEW."RoadCondition",
			NEW."Rate",
			NEW."ActivitiesRequired",
			NEW."SurfaceTypeId",
			current_timestamp,
			NEW."ID"

	);

	RETURN NEW;
--End of Dealing with the history table

END;
$BODY$
  LANGUAGE plpgsql;

CREATE TRIGGER roadconditioncodeunit_update_trigger
AFTER
UPDATE ON "public"."RoadConditionCodeUnit"
  FOR EACH ROW
EXECUTE PROCEDURE roadconditioncodeunit_update
();
