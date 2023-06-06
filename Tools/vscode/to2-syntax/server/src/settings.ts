export interface To2LspSettings {
  libraryPath: string;
  maxNumberOfProblems: number;
}

export const defaultSettings: To2LspSettings = {
  libraryPath: "",
  maxNumberOfProblems: 1000,
};
