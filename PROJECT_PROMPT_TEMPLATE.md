# 🚀 Projekt-Initiierungs-Template für /pm

> **Anleitung**: Fülle die Fragen unten aus und sende den ausgefüllten Text mit `/pm [dein Text]` ab. Der PM-Agent wird dann einen detaillierten Delegationsplan für dein Projekt erstellen.

---

## 1️⃣ Projektkontext

### PFLICHT: Was ist das Ziel dieses Projekts?
<!-- Beschreibe in 2-3 Sätzen, was du erreichen möchtest -->

**Beispiel:**
> Ich möchte eine Todo-App mit Firebase-Backend bauen, bei der Nutzer Aufgaben erstellen, bearbeiten und als erledigt markieren können. Die App soll als PWA funktionieren und offline nutzbar sein.

**Deine Antwort:**
> [Dein Projektziel hier]

---

### PFLICHT: Welche Hauptfunktionen soll das Projekt haben?
<!-- Liste die 3-5 wichtigsten Features auf -->

**Beispiel:**
> 1. Benutzer kann sich anonym authentifizieren
> 2. Benutzer kann Todos erstellen, bearbeiten, löschen
> 3. Todos werden in Firestore gespeichert und synchronisiert
> 4. Offline-First: Todos sind auch ohne Internet nutzbar
> 5. Dark Mode Toggle

**Deine Antwort:**
> [Liste deine Hauptfunktionen hier]

---

### OPTIONAL: Wer ist die Zielgruppe?
<!-- Hilft dem PM, UX-Entscheidungen zu treffen -->

**Beispiel:**
> Privatnutzer, die eine einfache, schnelle Todo-Verwaltung wollen. Keine technischen Power-User.

**Deine Antwort:**
> [Deine Zielgruppe oder "Keine Angabe"]

---

## 2️⃣ Feature-Definition

### PFLICHT: Was soll konkret gebaut/geändert werden?
<!-- Beschreibe die Aufgabe detailliert -->

**Beispiel:**
> Erstelle ein neues Feature "Kategorien". Nutzer sollen ihre Todos in Kategorien (z.B. "Arbeit", "Privat", "Einkaufen") gruppieren können. Jede Kategorie hat einen Namen und eine Farbe. Todos können beim Erstellen einer Kategorie zugeordnet werden. In der UI soll es einen Filter geben, um Todos nach Kategorie anzuzeigen.

**Deine Antwort:**
> [Deine Feature-Beschreibung hier]

---

### OPTIONAL: Gibt es ähnliche Referenzen oder Vorbilder?
<!-- z.B. "Wie in Trello" oder "Ähnlich wie Google Keep" -->

**Beispiel:**
> Die Kategorie-Ansicht soll ähnlich wie in Todoist funktionieren (Seitenleiste mit Kategorie-Liste).

**Deine Antwort:**
> [Referenzen oder "Keine Angabe"]

---

## 3️⃣ Technische Anforderungen

### PFLICHT: Welche Architektur-Schichten sind betroffen?
<!-- Markiere alle zutreffenden mit [x] -->

- [ ] `src/types/` - Neue Interfaces/Types definieren
- [ ] `src/core/` - Business Logic (pure functions)
- [ ] `src/services/` - Firebase/Firestore Integration
- [ ] `src/state/` - App State erweitern
- [ ] `src/ui/` - DOM Manipulation, Rendering
- [ ] `src/components/` - Neue Web Components
- [ ] `src/i18n/` - Neue Übersetzungen
- [ ] `src/styles/` - CSS/Styling Änderungen
- [ ] `index.html` - HTML Struktur anpassen
- [ ] Firestore Rules anpassen
- [ ] Andere: [Beschreibung]

---

### OPTIONAL: Gibt es spezielle technische Anforderungen?
<!-- z.B. "Muss mit großen Datensätzen umgehen können" oder "Performance-kritisch" -->

**Beispiel:**
> Die Kategorie-Filterung soll auch bei 1000+ Todos performant sein. Verwende Firestore Compound Queries.

**Deine Antwort:**
> [Spezielle Anforderungen oder "Keine"]

---

### OPTIONAL: Sollen bestehende Patterns/Komponenten wiederverwendet werden?
<!-- z.B. "Verwende das gleiche Modal-Pattern wie bei der Item-Bearbeitung" -->

**Beispiel:**
> Verwende die gleiche Card-Komponente (`app-card`) wie für die Todo-Items. Die Kategorie-Auswahl soll ein Dropdown sein (neue `app-select` Komponente nötig).

**Deine Antwort:**
> [Pattern-Wiederverwendung oder "PM entscheidet"]

---

## 4️⃣ UX/UI Anforderungen

### PFLICHT: Wie soll es aussehen/sich anfühlen?
<!-- Beschreibe das gewünschte Look & Feel -->

**Beispiel:**
> Minimalistisch und modern. Die Kategorie-Tags sollen als farbige Pills neben dem Todo-Text erscheinen. Die Farben sollen kräftig aber nicht grell sein. Animationen beim Filtern (smooth fade-in/out).

**Deine Antwort:**
> [Deine UX/UI Vision hier]

---

### OPTIONAL: Responsive Design Anforderungen?
<!-- Wie soll es auf Mobile/Tablet/Desktop aussehen? -->

**Beispiel:**
> Auf Mobile: Kategorie-Filter als Bottom Sheet. Auf Desktop: Seitenleiste links. Kategorien sollen auf allen Größen gut bedienbar sein.

**Deine Antwort:**
> [Responsive Anforderungen oder "Standard responsive"]

---

### OPTIONAL: Accessibility (A11y) Anforderungen?
<!-- z.B. "Muss screenreader-freundlich sein" -->

**Beispiel:**
> Kategorie-Filter muss per Tastatur bedienbar sein (Tab-Navigation). Farben müssen WCAG AA Kontrast erfüllen.

**Deine Antwort:**
> [A11y Anforderungen oder "Standard WCAG AA"]

---

## 5️⃣ Qualitätsanforderungen

### PFLICHT: Welche Tests werden erwartet?
<!-- Markiere alle zutreffenden mit [x] -->

- [ ] Unit Tests für Business Logic (`tests/unit/`)
- [ ] Integration Tests für Services (`tests/integration/`)
- [ ] Component Tests für Web Components
- [ ] E2E Tests (falls vorhanden)
- [ ] Manuelle Tests ausreichend

**Zusätzliche Test-Anforderungen:**
> [z.B. "Edge Cases: leere Kategorie, sehr langer Kategoriename, 100+ Kategorien"]

---

### PFLICHT: Welche Qualitätschecks sollen laufen?
<!-- Standardmäßig immer: typecheck, lint, test, build -->

- [x] `npm run typecheck` - TypeScript Fehlerfreiheit
- [x] `npm test -- --run` - Alle Tests bestehen
- [x] `npm run lint` - ESLint ohne Fehler
- [x] `npm run build` - Production Build erfolgreich
- [ ] Zusätzlich: [andere Checks]

---

### OPTIONAL: Coverage-Ziel?
<!-- z.B. "80% line coverage für neue Module" -->

**Deine Antwort:**
> [Coverage-Ziel oder "Standard (80%+)"]

---

## 6️⃣ Einschränkungen & No-Gos

### OPTIONAL: Was soll NICHT gemacht werden?
<!-- Hilft dem PM, Scope-Creep zu vermeiden -->

**Beispiel:**
> - Keine Server-seitige Kategorievalidierung (Client-Side reicht)
> - Keine Kategorie-Hierarchien (nur flache Liste)
> - Keine Kategorie-Sharing zwischen Nutzern

**Deine Antwort:**
> [Einschränkungen oder "Keine speziellen Einschränkungen"]

---

### OPTIONAL: Gibt es Deadlines oder Zeitdruck?
<!-- Hilft dem PM bei Prioritäts-Entscheidungen -->

**Beispiel:**
> MVP soll in 2 Tagen fertig sein. Nice-to-have Features können später kommen.

**Deine Antwort:**
> [Zeitliche Einschränkungen oder "Keine Deadline"]

---

## 7️⃣ Abschluss & Prioritäten

### PFLICHT: Wann ist das Feature "fertig"?
<!-- Definition of Done -->

**Beispiel:**
> Das Feature ist fertig, wenn:
> 1. Nutzer können Kategorien erstellen, bearbeiten, löschen
> 2. Todos können einer Kategorie zugeordnet werden
> 3. Filter funktioniert (zeigt nur Todos der gewählten Kategorie)
> 4. Alle Tests laufen durch
> 5. TypeScript Build ohne Fehler
> 6. UI ist responsive (Mobile + Desktop)
> 7. Deutsche und englische i18n Texte vorhanden

**Deine Antwort:**
> [Deine Definition of Done hier]

---

### OPTIONAL: Prioritäten bei Tradeoffs?
<!-- Was ist wichtiger? -->

**Beispiel:**
> Priorität 1: Funktionalität (muss alles klappen)
> Priorität 2: Code-Qualität (saubere Architektur)
> Priorität 3: Pixel-Perfect Design (nice-to-have)

**Deine Antwort:**
> [Deine Prioritäten oder "PM entscheidet"]

---

## ✅ Bereit zum Absenden?

### Checkliste:
- [ ] Projektziel beschrieben (Sektion 1)
- [ ] Hauptfunktionen gelistet (Sektion 1)
- [ ] Feature detailliert beschrieben (Sektion 2)
- [ ] Betroffene Architektur-Schichten markiert (Sektion 3)
- [ ] UX/UI Vision beschrieben (Sektion 4)
- [ ] Test-Anforderungen markiert (Sektion 5)
- [ ] Definition of Done formuliert (Sektion 7)

### 🚀 So geht's weiter:

**Option 1: Vollständiger Prompt**
Kopiere deinen ausgefüllten Text und sende:
```
/pm [dein vollständig ausgefüllter Prompt von oben]
```

**Option 2: Kompakter Prompt**
Wenn du nur die wichtigsten Infos senden willst:
```
/pm Erstelle [Feature-Name].

Ziel: [Kurzbeschreibung]

Features:
- [Feature 1]
- [Feature 2]

Technisch:
- [Betroffene Schichten]

UX: [Look & Feel]

Done wenn:
- [Kriterium 1]
- [Kriterium 2]
```

---

## 📚 Tipps für bessere Ergebnisse

### ✅ Gut:
- Konkrete, messbare Akzeptanzkriterien
- Klare Beschreibung der Nutzer-Perspektive ("Nutzer kann X tun")
- Referenzen zu existierenden Patterns im Projekt
- Realistische Scope-Definition

### ❌ Vermeiden:
- Vage Beschreibungen ("Mach es schön")
- Zu große Features ohne Aufteilung
- Widersprüchliche Anforderungen
- Technische Details, die der PM besser entscheiden kann

---

## 🎯 Beispiel: Vollständig ausgefüllter Prompt

<details>
<summary>Klicke hier für ein vollständiges Beispiel</summary>

```
/pm Erstelle ein neues Feature "Kategorien" für die Todo-App.

## Projektziel
Nutzer sollen ihre Todos in Kategorien gruppieren können, um bessere Übersicht zu haben.

## Hauptfunktionen
1. Kategorien erstellen, bearbeiten, löschen
2. Todos einer Kategorie zuordnen
3. Nach Kategorie filtern
4. Kategorie hat Name + Farbe

## Feature-Details
Nutzer können im Hauptmenü "Neue Kategorie" klicken. Es öffnet sich ein Modal mit Eingabefeldern für Name (max 30 Zeichen) und Farbwahl (6 vordefinierte Farben). Nach Speichern erscheint die Kategorie in der Seitenleiste. Beim Todo-Erstellen kann eine Kategorie aus Dropdown gewählt werden. Click auf Kategorie in Seitenleiste filtert die Todo-Liste.

## Betroffene Schichten
- [x] src/types/ - CategoryInterface
- [x] src/core/ - validateCategory()
- [x] src/services/ - category-service.ts (CRUD)
- [x] src/state/ - categories[] hinzufügen
- [x] src/ui/ - Kategorie-UI Rendering
- [x] src/components/ - app-category-pill, app-category-modal
- [x] src/i18n/ - Kategorie-Texte (DE/EN)
- [x] Firestore Rules - categories Collection

## UX/UI
Minimalistisch. Kategorie-Pills als farbige Badges neben Todo-Text. Seitenleiste links (Desktop) / Bottom Sheet (Mobile). Smooth Animationen beim Filtern. WCAG AA Kontrast für alle Farben.

## Tests
- [x] Unit Tests für validateCategory()
- [x] Integration Tests für category-service
- [x] Component Tests für app-category-pill
Edge Cases: leerer Name, sehr langer Name, 50+ Kategorien, doppelte Namen

## Done wenn
1. CRUD für Kategorien funktioniert
2. Todo-Kategorie-Zuordnung funktioniert
3. Filter zeigt korrekte Todos
4. Tests: typecheck + test + build grün
5. Responsive (Mobile + Desktop)
6. i18n DE + EN
7. Firestore Rules schützen Kategorien

## Prioritäten
1. Funktionalität
2. Tests
3. Code-Qualität
4. Pixel-Perfect Design

## Einschränkungen
- Keine Kategorie-Hierarchien (nur flach)
- Keine Sharing zwischen Nutzern
- Max 50 Kategorien pro Nutzer
```

</details>

---

**Version:** 1.0
**Letzte Aktualisierung:** 2026-02-12
**Kompatibel mit:** Vite + Firebase + PWA Template
