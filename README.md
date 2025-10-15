# Simply-Calc_AppWPF
# Simply Calc - Calculatrice Simplifié WPF

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=.net)
![WPF](https://img.shields.io/badge/WPF-Windows-0078D4?logo=windows)
![C#](https://img.shields.io/badge/C%23-12.0-239120?logo=csharp)
![License](https://img.shields.io/badge/license-MIT-green)

Une calculatrice scientifique moderne développée en WPF (.NET 8.0) avec une architecture propre et maintenable utilisant le pattern **Partial Classes** pour une séparation optimale des responsabilités.

## Table des matières

- [Aperçu](##-aperçu)
- [Fonctionnalités](##-fonctionnalités)
- [Architecture](##-architecture)
- [Structure du projet](##-structure-du-projet)
- [Installation](##-installation)
- [Utilisation](##-utilisation)
- [Technologies](##-technologies)
- [Contribution](##-contribution)

## Aperçu

Simply Calc est une calculatrice scientifique Simplifié offrant des opérations arithmétiques de base, des fonctions trigonométriques et la gestion de constantes mathématiques. L'application suit les principes SOLID et utilise une architecture modulaire pour faciliter la maintenance et l'évolution.

### Captures d'écran

```
┌─────────────────────────────────────┐
│  Simply Calc - v1.0.0               │
├─────────────────────────────────────┤
│  sin(45)+cos(30)×π                  │  ← Zone d'opération
│  = 4.23594                          │  ← Résultat intermédiaire
│  4.23594                            │  ← Résultat principal
├─────────────────────────────────────┤
│  [Sin] [Cos] [Tan] [CE]  [C]       │
│  [Arc] [Arc] [Arc] [Bck] [÷]       │
│  [ π ] [ 7 ] [ 8 ] [ 9 ] [×]       │
│  [ e ] [ 4 ] [ 5 ] [ 6 ] [-]       │
│  [+/-] [ 1 ] [ 2 ] [ 3 ] [+]       │
│  [+/-] [ 0 ] [ . ] [   =   ]       │
└─────────────────────────────────────┘
```

## Fonctionnalités

### Opérations de base
-  Addition, soustraction, multiplication, division
- Gestion des nombres décimaux
- Changement de signe (+/-)
- Effacement (CE / C / Back)

### Fonctions scientifiques
- **Trigonométrie directe** : sin, cos, tan (en degrés)
- **Trigonométrie inverse** : arcsin, arccos, arctan
- **Constantes** : π (Pi), e (Euler)
- Conversion automatique degrés ↔ radians

### Interface utilisateur
-  Interface WPF moderne et responsive
- Ajustement dynamique de la taille de police
- Gestion des erreurs avec affichage visuel
- Support clavier complet
-  Thème sombre élégant

### Fonctionnalités avancées
- Historique des calculs avec export CSV/JSON/TXT
- Validation des expressions en temps réel
- Gestion des parenthèses automatique
- Calcul en chaîne
- Logs d'erreurs détaillés

## Architecture

### Pattern Partial Classes

L'application utilise intensivement les **partial classes** pour séparer les responsabilités de la fenêtre principale :

```
MainWindow
├── MainWindow.xaml.cs          (Constructeur et initialisation)
├── MainWindowState.cs          (État et services)
├── MainWindowOperator.cs       (Opérateurs arithmétiques)
├── MainWindowFunction.cs       (Fonctions scientifiques)
├── MainWindowSpecialKey.cs     (Touches spéciales)
├── MainWindowCompute.cs        (Moteur de calcul)
├── MainWindowDisplay.cs        (Gestion de l'affichage)
└── MainWindowEventHandler.cs   (Gestionnaires d'événements)
```

### Principes de conception

- **Separation of Concerns** : Chaque partial class a une responsabilité unique
- **DRY (Don't Repeat Yourself)** : Code réutilisable via helpers et services
- **Single Responsibility Principle** : Classes cohésives avec un seul objectif
- **Dependency Injection** : Services injectés via constructeur
- **Error Handling** : Gestion centralisée des erreurs

## Structure du projet

```
Simply-Calc_AppWPF/
│
├── 📂 core/                          # Logique métier principale
│   ├── CalculatorEngine.cs          # Moteur de calcul
│   ├── Constants.cs                 # Constantes globales
│   └── OperationHistory.cs          # Historique des opérations
│
├── 📂 Models/                        # Modèles de données
│   ├── CalculatorState.cs           # État de la calculatrice
│   └── OperatorType.cs              # Types d'opérateurs
│
├── 📂 Services/                      # Services applicatifs
│   ├── ErrorHandler.cs              # Gestion des erreurs
│   ├── ExpressionEvaluator.cs       # Évaluation d'expressions
│   └── FunctionRegistry.cs          # Registre des fonctions
│
├── 📂 Helpers/                       # Classes utilitaires
│   ├── FormattingHelper.cs          # Formatage des nombres
│   ├── MathHelper.cs                # Opérations mathématiques
│   └── ParsingHelper.cs             # Parsing et validation
│
├── 📂 UI/                            # Partial classes UI
│   ├── MainWindowState.cs           # Variables d'état
│   ├── MainWindowOperator.cs        # Logique opérateurs
│   ├── MainWindowFunction.cs        # Logique fonctions
│   ├── MainWindowSpecialKey.cs      # Touches spéciales
│   ├── MainWindowCompute.cs         # Calculs
│   ├── MainWindowDisplay.cs         # Affichage
│   └── MainWindowEventHandler.cs    # Événements
│
├── MainWindow.xaml                   # Interface XAML
├── MainWindow.xaml.cs                # Code-behind principal
├── App.xaml                          # Configuration application
├── App.xaml.cs                       # Point d'entrée
└── README.md                         # Documentation
```

## Installation

### Prérequis

- **Windows 10/11** (64-bit)
- **.NET 8.0 SDK** ou supérieur
- **Visual Studio 2022** (recommandé) ou VS Code

### Étapes d'installation

1. **Cloner le dépôt**
```bash
git clone https://github.com/kryssSampi/Simply-Calc_AppWPF.git
cd Simply-Calc_AppWPF
```

2. **Restaurer les packages NuGet**
```bash
dotnet restore
```

3. **Compiler le projet**
```bash
dotnet build --configuration Release
```

4. **Exécuter l'application**
```bash
dotnet run
```

### Installation via Visual Studio

1. Ouvrir `Simply-Calc_AppWPF.sln` dans Visual Studio 2022
2. Appuyer sur `F5` pour compiler et exécuter

## Utilisation

### Opérations de base

```
Exemples :
  5 + 3         →  8
  10 - 4        →  6
  7 × 8         →  56
  20 ÷ 4        →  5
  -5 + 3        →  -2
```

### Fonctions scientifiques

```
Exemples :
  sin(30)       →  0.5
  cos(60)       →  0.5
  tan(45)       →  1
  arcsin(0.5)   →  30
  π × 2         →  6.283185
  e × 3         →  8.154845
```

### Expressions complexes

```
Exemples :
  sin(45) + cos(30) × π    →  4.23594
  3 × (4 + 5)              →  27
  arctan(1) × 4            →  180
```

### Raccourcis clavier

| Touche | Action |
|--------|--------|
| `0-9` | Saisie de chiffres |
| `+ - * /` | Opérateurs |
| `.` ou `,` | Point décimal |
| `Enter` | Égal (=) |
| `Backspace` | Supprimer dernier caractère |
| `Delete` | Clear Entry (CE) |
| `Escape` | Clear All (C) |
| `F9` | Changement de signe |
| `Ctrl+S` | sin |
| `Ctrl+C` | cos |
| `Ctrl+T` | tan |
| `Ctrl+P` | π |
| `Ctrl+E` | e |

## Technologies

### Framework et langages
- **.NET 8.0** - Framework applicatif
- **C# 12.0** - Langage de programmation
- **WPF (Windows Presentation Foundation)** - Interface utilisateur
- **XAML** - Markup interface

### Librairies
- `System.Windows` - Framework WPF
- `System.Globalization` - Gestion des formats numériques
- `System.IO` - Gestion des fichiers (logs, exports)

### Patterns et concepts
- **Partial Classes** - Séparation des responsabilités
- **MVVM-like** - État séparé de la logique UI
- **Service Pattern** - Services réutilisables
- **Repository Pattern** - Historique des opérations
- **Strategy Pattern** - Fonctions et opérateurs

## Diagrammes

### Flux de calcul

```
┌──────────────┐
│ Saisie User  │
└──────┬───────┘
       │
       ▼
┌──────────────────┐
│ Validation Input │
└──────┬───────────┘
       │
       ▼
┌─────────────────┐
│ Parsing/Tokenize│
└──────┬──────────┘
       │
       ▼
┌──────────────────┐
│ CalculatorEngine │
└──────┬───────────┘
       │
       ▼
┌───────────────┐
│ Formatage     │
└──────┬────────┘
       │
       ▼
┌──────────────┐
│ Affichage    │
└──────────────┘
```

### Classes principales

```
┌─────────────────────┐
│   MainWindow        │
│ ─────────────────── │
│ + CalculatorState   │
│ + CalculatorEngine  │
│ + ErrorHandler      │
│ + OperationHistory  │
└──────────┬──────────┘
           │
           │ utilise
           │
           ▼
┌─────────────────────────────┐
│     CalculatorEngine        │
│ ─────────────────────────── │
│ + Calculate()               │
│ + ApplyFunction()           │
│ + ValidateOperation()       │
└─────────────────────────────┘
```

## Tests

```bash
# Exécuter les tests unitaires (à venir)
dotnet test
```

## Logs et debugging

L'application génère automatiquement des logs dans :
```
%AppData%/CalculatriceWPF/
├── errors.log      # Logs d'erreurs
└── history.csv     # Historique exporté
```

## Contribution

Les contributions sont les bienvenues ! Voici comment contribuer :

1. **Fork** le projet
2. Créer une **branche** (`git checkout -b feature/AmazingFeature`)
3. **Commit** les changements (`git commit -m 'Add AmazingFeature'`)
4. **Push** vers la branche (`git push origin feature/AmazingFeature`)
5. Ouvrir une **Pull Request**

### Guidelines de code

- Suivre les conventions de nommage C#
- Documenter avec XML comments
- Respecter l'architecture en partial classes
- Ajouter des tests unitaires pour les nouvelles fonctionnalités

## Licence

Ce projet est sous licence **MIT** - voir le fichier [LICENSE](LICENSE) pour plus de détails.

## Auteur

 Moi ``Kryss Nana``
- GitHub: [@votre-username](https://github.com/KryssSampi)
- Email: [Kryss Nana](SampiKryss@gmail.com)

## Remerciements

- Microsoft pour le framework .NET et WPF
- La communauté C# pour les ressources et l'inspiration
- Tous les contributeurs du projet

## Roadmap

### Version 1.1 (À venir)
- [ ] Mode scientifique avancé (log, ln, exp, puissance)
- [ ] Mode programmeur (binaire, hexadécimal)
- [ ] Thèmes personnalisables
- [ ] Support multi-langue

### Version 1.2 (Futur)
- [ ] Graphiques de fonctions
- [ ] Résolution d'équations
- [ ] Calcul matriciel
- [ ] Mode RPN (Notation Polonaise Inverse)

## Problèmes connus

- La division par zéro est gérée mais affiche "Impossible"
- Les très grands nombres passent en notation scientifique
- Le support des parenthèses imbriquées est limité à un niveau

## Support

Pour toute question ou problème :
- Ouvrir une [issue](https://github.com/votre-username/Simply-Calc_AppWPF/issues)
- Consulter la [documentation](https://github.com/votre-username/Simply-Calc_AppWPF/wiki)

---

**⭐ Si vous aimez ce projet, n'hésitez pas à lui donner une étoile !**

*Fait avec ❤️ en C# et WPF*