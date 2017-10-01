rm(list=ls())

args = commandArgs(trailingOnly = TRUE) # allows R to get parameters

if (!require(mvtnorm)) install.packages("mvtnorm")
if (!require(ICSNP)) install.packages("ICSNP")

library(mvtnorm)
library(ICSNP)
#ElecData <- read.csv('d:/ann_project_aut_sem3/microsoft/backpropprogram/hotellingr/elecdata.csv')
#irisRem <- ElecData

filePathAndName <- args[2] #d:/ann_project_aut_sem3/microsoft/backpropprogram/hotellingr/Data_withYEle.csv
ElecData <- read.csv(filePathAndName)
totalCols <- ncol(ElecData)
irisRem <- ElecData[, 1:totalCols - 1]


# one sample test
#data(iris)
#HotellingsT2(irisRem)
#HotellingsT2(irisRem,mu=c(0,0,0,0), test="chi")


## Two sample test
## 75% of the sample size
p_size = as.numeric(args[1]) #0.2 #enter train size
smp_size <- floor(p_size * nrow(irisRem))

## set the seed to make your partition reproductible
set.seed(123)
train_ind <- sample(seq_len(nrow(irisRem)), size = smp_size)

trainRem <- irisRem[train_ind,]
testRem <- irisRem[-train_ind,]


fit = HotellingsT2(trainRem, testRem)
fit$p.value


#train = train[,1:4]
#test = test[,1:4]

#HotellingsT2(data(iris))
#Z<-rbind(trainRem,testRem)
#g<-factor(rep(c(1,2),c(15,135)))
#HotellingsT2(Z~g, mu = rep(-0.5,4))