CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE height (
    id integer PRIMARY KEY,
    cm integer NOT NULL
);

CREATE TABLE width (
    id integer PRIMARY KEY,
    cm integer NOT NULL
);

CREATE TABLE category (
    id integer PRIMARY KEY,
    cat text NOT NULL
);

INSERT INTO category (id, cat) VALUES
  (1, 'Cute animals'),
  (2, 'Fairytales'),
  (3, 'Animal planet'),
  (4, 'Flowers')
ON CONFLICT (id) DO NOTHING;

CREATE TABLE paintings (
    id uuid PRIMARY KEY,
    height_id integer,
    width_id integer,
    category_id integer,
    name text,
    image_link text
);

CREATE INDEX "IX_paintings_height_id" ON paintings (height_id);

CREATE INDEX "IX_paintings_width_id" ON paintings (width_id);

CREATE INDEX "IX_paintings_category_id" ON paintings (category_id);

ALTER TABLE paintings
ADD CONSTRAINT IF NOT EXISTS "FK_paintings_height_height_id"
FOREIGN KEY (height_id) REFERENCES height (id) ON DELETE CASCADE;

ALTER TABLE paintings
ADD CONSTRAINT IF NOT EXISTS "FK_paintings_width_width_id"
FOREIGN KEY (width_id) REFERENCES width (id) ON DELETE CASCADE;

ALTER TABLE paintings
ADD CONSTRAINT IF NOT EXISTS "FK_paintings_category_category_id"
FOREIGN KEY (category_id) REFERENCES category (id) ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260119082043_InitialCreate', '10.0.0')
ON CONFLICT DO NOTHING;

