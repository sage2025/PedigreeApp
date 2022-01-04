export const START_YEAR = 1670;

export const RACE_COUNTRIES = ['ARGENTINA', 'AUSTRALIA', 'BRAZIL', 'CANADA', 'CHILE', 'FRANCE', 'GERMANY', 'SCANDINAVIA', 'HONG KONG', 'IRELAND', 'ITALY', 'JAPAN', 'NEW ZEALAND', 'PERU', 'QATAR', 'SINGAPORE', 'SOUTH AFRICA', 'TURKEY', 'UNITED ARAB EMIRATES', 'UNITED KINGDOM', 'UNITED STATES', 'URUGUAY', 'ZIMBABWE', 'OTHER'];

export const RACE_DISTANCES = [
    { value: 'Sprint', label: 'Sprint (1000m-1300m)' },
    { value: 'Sprinter/Miler', label: 'Sprinter/Miler (1301 - 1899m)' },
    { value: 'Intermediate', label: 'Intermediate (1900m-2050m)' },
    { value: 'Long', label: 'Long (2051m-2700m)' },
    { value: 'Extended', label: 'Extended (2701m+)' },
];

export const RACE_SURFACES = ['Turf', 'Dirt', 'Artificial'];

export const RACE_TYPES = ['2YO +', '2YO only', '2YO Colts', '2YO Fillies', '3YOs only', '3YO Fillies', '3YO C&G', '3YO +', '3YO + F&M', '4YO +', '4YO+ Mares only', 'WFA Open'];

export const RACE_STATUSES = ['G1', 'G2', 'G3', 'LR'];

export const GROUP_COLORS = [
    {name: 'Red', value: '#e6194B', textColor: 'white'},
    {name: 'Orange', value: '#f58231', textColor: 'white'},
    {name: 'Yellow', value: '#ffe119', textColor: 'black'},
    {name: 'Lime', value: '#bfef45', textColor: 'black'},
    {name: 'Green', value: '#3cb44b', textColor: 'white'},
    {name: 'Cyan', value: '#42d4f4', textColor: 'black'},
    {name: 'Blue', value: '#4363d8', textColor: 'white'},
    {name: 'Purple', value: '#911eb4', textColor: 'white'},
    {name: 'Margenta', value: '#f032e6', textColor: 'white'},
    {name: 'Maroon', value: '#800000', textColor: 'white'},
    {name: 'Brown', value: '#9A6324', textColor: 'white'},
    {name: 'Olive', value: '#808000', textColor: 'white'},
    {name: 'Teal', value: '#469990', textColor: 'white'},
    {name: 'Navy', value: '#000075', textColor: 'white'},
    {name: 'Pink', value: '#fabed4', textColor: 'black'},
    {name: 'Apricot', value: '#ffd8b1', textColor: 'black'},
    {name: 'Mint', value: '#aaffc3', textColor: 'black'},
    {name: 'Lavender', value: '#dcbeff', textColor: 'black'},
]



export const ML_COLUMNS: object[] = [
    { category: 'Subject Horse', name: 'MtDNAGroups', description: '', checked: true },
    { category: 'Subject Horse', name: 'AHC', description: '', checked: true },
    { category: 'Subject Horse', name: 'Bal', description: '', checked: true },
    { category: 'Subject Horse', name: 'Kal', description: '', checked: true },

    { category: 'Subject Horse', name: 'COI', description: '', checked: true },
    //{ category: 'Subject Horse', name: 'MtDNAGroup', description: '' },
    { category: 'Subject Horse', name: 'COI1', description: '' },
    { category: 'Subject Horse', name: 'COI2', description: '' },
    { category: 'Subject Horse', name: 'COI3', description: '' },
    { category: 'Subject Horse', name: 'COI4', description: '' },
    { category: 'Subject Horse', name: 'COI5', description: '' },
    { category: 'Subject Horse', name: 'COI6', description: '' },
    { category: 'Subject Horse', name: 'COI7', description: '' },
    { category: 'Subject Horse', name: 'COI8', description: '' },

    { category: 'Subject Horse', name: 'COID1', description: '' },
    { category: 'Subject Horse', name: 'COID2', description: '' },
    { category: 'Subject Horse', name: 'COID3', description: '' },
    { category: 'Subject Horse', name: 'COID4', description: '' },
    { category: 'Subject Horse', name: 'COID5', description: '' },
    { category: 'Subject Horse', name: 'COID6', description: '' },

    { category: 'Subject Horse', name: 'GI', description: '' },
    { category: 'Subject Horse', name: 'GDGS', description: '' },
    { category: 'Subject Horse', name: 'GDGD', description: '' },
    { category: 'Subject Horse', name: 'GSSD', description: '' },
    { category: 'Subject Horse', name: 'GSDD', description: '' },
    { category: 'Subject Horse', name: 'UniqueAncestorsCount', description: '' },
    { category: 'Subject Horse', name: 'Pedigcomp', description: '' },


    { category: 'Sire', name: 'SireHistoricalBPR', description: '' },
    { category: 'Sire', name: 'SireZHistoricalBPR', description: '' },
    { category: 'Sire', name: 'SireCOI', description: '' },
    { category: 'Sire', name: 'SireAHC', description: '' },
    { category: 'Sire', name: 'SireBal', description: '' },
    { category: 'Sire', name: 'SireKal', description: '' },
    { category: 'Sire', name: 'SireHistoricalSR', description: '' },

    { category: 'Sire', name: 'SireCurrentIV', description: '' },
    { category: 'Sire', name: 'SireCurrentAE', description: '' },
    { category: 'Sire', name: 'SireCurrentPRB2', description: '' },
    { category: 'Sire', name: 'SireHistoricalIV', description: '' },
    { category: 'Sire', name: 'SireHistoricalAE', description: '' },
    { category: 'Sire', name: 'SireHistoricalPRB2', description: '' },

    { category: 'Dam', name: 'DamHistoricalBPR', description: '' },
    { category: 'Dam', name: 'DamZHistoricalBPR', description: '' },
    { category: 'Dam', name: 'DamCOI', description: '' },
    { category: 'Dam', name: 'DamAHC', description: '' },
    { category: 'Dam', name: 'DamBal', description: '' },
    { category: 'Dam', name: 'DamKal', description: '' },

    { category: 'GrandSire', name: 'GrandSireHistoricalBPR', description: '' },
    { category: 'GrandSire', name: 'GrandSireZHistoricalBPR', description: '' },
    { category: 'GrandSire', name: 'GrandSireCOI', description: '' },
    { category: 'GrandSire', name: 'GrandSireAHC', description: '' },
    { category: 'GrandSire', name: 'GrandSireBal', description: '' },
    { category: 'GrandSire', name: 'GrandSireKal', description: '' },
    { category: 'GrandSire', name: 'GrandSireHistoricalSR', description: '' },
    { category: 'GrandSire', name: 'GrandSireSOSHistoricalSR', description: '' },

    { category: 'BroodmareSire', name: 'BroodmareSireHistoricalBPR', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireZHistoricalBPR', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireCOI', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireAHC', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireBal', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireKal', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireHistoricalSR', description: '' },
    { category: 'BroodmareSire', name: 'BroodmareSireBMSHistoricalSR', description: '' },

    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireHistoricalBPR', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireZHistoricalBPR', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireCOI', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireAHC', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireBal', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireKal', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireHistoricalSR', description: '' },
    { category: 'BroodmareSireOfSire', name: 'BroodmareSireOfSireBMSHistoricalSR', description: '' },
    
    { category: 'GrandDam', name: 'GrandDamHistoricalBPR', description: '' },
    { category: 'GrandDam', name: 'GrandDamZHistoricalBPR', description: '' },
    { category: 'GrandDam', name: 'GrandDamCOI', description: '' },
    { category: 'GrandDam', name: 'GrandDamAHC', description: '' },
    { category: 'GrandDam', name: 'GrandDamBal', description: '' },
    { category: 'GrandDam', name: 'GrandDamKal', description: '' },
];
